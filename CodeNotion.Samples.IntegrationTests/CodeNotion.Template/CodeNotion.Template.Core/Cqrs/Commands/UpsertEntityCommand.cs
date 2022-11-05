using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CodeNotion.Template.Business.Enums;
using CodeNotion.Template.Business.Exceptions;
using CodeNotion.Template.Business.Extensions;
using CodeNotion.Template.Data.SqlServer.Models.Interfaces;
using CodeNotion.Template.Data.SqlServer.Persistence;

namespace CodeNotion.Template.Business.Cqrs.Commands;

public class UpsertEntityCommand<TEntity> : IRequest<TEntity> where TEntity : class, IEntity
{
    public TEntity? Entity { get; set; }
    public Type[] ModifiedTypes => new[] {typeof(TEntity)};

    public UpsertEntityCommand(TEntity? entity)
    {
        Entity = entity;
    }
}

internal class BaseUpsertEntityCommandHandler // keep this non-generic
{
    // Tiene traccia delle entità già aggiornate con lo scopo di evitare circoli di update ricorsivi:
    // Es: Entità A contiene un riferimento B, B contiene un riferimento C, C contiene un riferimento ad A
    protected static readonly AsyncLocal<List<object>> UpsertedEntities = new();
}

internal class UpsertEntityCommandHandler<TEntity> : BaseUpsertEntityCommandHandler, IRequestHandler<UpsertEntityCommand<TEntity>, TEntity>
    where TEntity : class, IEntity
{
    protected static readonly MethodInfo UpsertCollectionMethod = typeof(UpsertEntityCommandHandler<TEntity>).GetMethod(nameof(UpsertCollection), BindingFlags.Instance | BindingFlags.NonPublic)!;
    protected readonly FullCodeNotionTemplateContext Context;
    protected readonly IServiceProvider Provider;
    private readonly IMediator _mediator;

    protected static readonly PropertyInfo[] RelatedEntities = typeof(TEntity)
        .GetProperties()
        .Where(x => !x.PropertyType.IsSimple())
        .Where(x => Nullable.GetUnderlyingType(x.PropertyType) == null)
        .ToArray();

    public UpsertEntityCommandHandler(FullCodeNotionTemplateContext context, IServiceProvider provider, IMediator mediator)
    {
        Context = context;
        Provider = provider;
        _mediator = mediator;

        UpsertedEntities.Value ??= new List<object>();
    }

    public virtual async Task<TEntity> Handle(UpsertEntityCommand<TEntity> request, CancellationToken ct)
    {
        if (request.Entity == null)
        {
            throw new BadRequestException(ErrorType.InvalidSchema, "Il dato fornito non è presente o non conforme allo schema previsto");
        }

        UpsertedEntities.Value!.Add(request.Entity);
        await ValidateAsync(request.Entity, ct);

        Context.Entry(request.Entity).State = request.Entity.Id <= 0 ? EntityState.Added : EntityState.Modified;

        await UpdateRelatedEntitiesAsync(request.Entity, ct);

        await Context.SaveChangesAsync(ct);
        return request.Entity;
    }

    private async Task UpdateRelatedEntitiesAsync(TEntity requestEntity, CancellationToken ct)
    {
        foreach (var relatedEntityPropertyInfo in RelatedEntities)
        {
            var relatedEntity = relatedEntityPropertyInfo.GetValue(requestEntity);
            switch (relatedEntity)
            {
                case null:
                    continue;
                // ReSharper disable once InvertIf
                case IEnumerable enumerable:
                {
                    var relatedCollectionEntityType = relatedEntityPropertyInfo.PropertyType.GenericTypeArguments[0];
                    if (!typeof(IEntity).IsAssignableFrom(relatedCollectionEntityType))
                    {
                        continue; // Attempting to edit a non IEntity child collection.
                    }

                    await ((Task) UpsertCollectionMethod
                        .MakeGenericMethod(typeof(TEntity), relatedCollectionEntityType)
                        .Invoke(this, new object[] {requestEntity, relatedEntityPropertyInfo, enumerable, ct})!)!;

                    // ReSharper disable once RedundantJumpStatement
                    continue;
                }
                case IEntity relatedIEntity:
                    if (ShouldSkipUpsert(relatedIEntity))
                    {
                        break; // evita ricorsività
                    }

                    var relatedEntityType = relatedIEntity.GetType();
                    var commandType = typeof(UpsertEntityCommand<>).MakeGenericType(relatedEntityType);
                    await _mediator.Send(Activator.CreateInstance(commandType, relatedIEntity)!, ct);
                    break;
                default:
                    Context.Update(relatedEntity);
                    break;
            }
        }
    }

#pragma warning disable 693
    private async Task UpsertCollection<TEntity, TChildEntity>(TEntity entity, PropertyInfo propertyInfo, IReadOnlyCollection<TChildEntity> editedEntries, CancellationToken ct)
        where TChildEntity : class, IEntity
    {
        var query = (IQueryable<TChildEntity>) Context
            .Entry(entity!)
            .Collection(propertyInfo.Name)
            .Query();

        var dbIds = await query.Select(x => x.Id).ToArrayAsync(ct);

        var deletedEntityIds = dbIds.Where(x => !editedEntries.Select(e => e.Id).Contains(x));
        foreach (var deletedEntityId in deletedEntityIds)
        {
            await _mediator.Send(new DeleteEntityByIdCommand<TChildEntity>(deletedEntityId), ct);
        }

        foreach (var editedEntity in editedEntries)
        {
            if (ShouldSkipUpsert(editedEntity))
            {
                continue; // evita ricorsività
            }

            await _mediator.Send(new UpsertEntityCommand<TChildEntity>(editedEntity), ct);
        }
    }

    protected virtual async Task ValidateAsync(TEntity entity, CancellationToken ct)
    {
        using var scope = Provider.CreateScope();
        var validator = scope.ServiceProvider.GetService<AbstractValidator<TEntity>>();
        if (validator == null)
        {
            return;
        }

        var validation = await validator.ValidateAsync(entity, ct);
        if (!validation.IsValid)
        {
            var code = validation.Errors.First().ErrorCode;
            throw new BadRequestException(Enum.Parse<ErrorType>(code), "Errore di validazione");
        }
    }

    private static bool ShouldSkipUpsert(IEntity relatedIEntity) =>
        UpsertedEntities.Value!.Contains(relatedIEntity);
}