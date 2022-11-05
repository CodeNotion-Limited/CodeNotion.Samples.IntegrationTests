using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using CodeNotion.Template.Business.Enums;
using CodeNotion.Template.Business.Exceptions;
using CodeNotion.Template.Business.Extensions;
using CodeNotion.Template.Data.SqlServer.Models.Interfaces;
using CodeNotion.Template.Data.SqlServer.Persistence;

namespace CodeNotion.Template.Business.Cqrs.Commands;

public class DeleteEntityByIdCommand<TEntity> : IRequest<int> where TEntity : class, IEntity
{
    public readonly int Id;
    public TEntity? Entity { get; set; }

    public DeleteEntityByIdCommand(int id)
    {
        Id = id;
    }
}

internal class DeleteEntityByIdCommandHandler<TEntity> : IRequestHandler<DeleteEntityByIdCommand<TEntity>, int>
    where TEntity : class, IEntity
{
    protected readonly FullCodeNotionTemplateContext Context;
    protected readonly Dictionary<Type, List<int>> FoundDependencies = new();

    protected static readonly PropertyInfo[] RelatedEntities = typeof(TEntity)
        .GetProperties()
        .Where(x => !x.PropertyType.IsSimple())
        .Where(x => Nullable.GetUnderlyingType(x.PropertyType) == null)
        .Where(x => typeof(IEnumerable).IsAssignableFrom(x!.PropertyType))
        .ToArray();

    public DeleteEntityByIdCommandHandler(FullCodeNotionTemplateContext context)
    {
        Context = context;
    }

    public virtual async Task<int> Handle(DeleteEntityByIdCommand<TEntity> request, CancellationToken ct)
    {
        if (request.Id <= 0)
        {
            throw new BadRequestException(ErrorType.InvalidId);
        }

        var entity = await Context.Set<TEntity>().SingleOrDefaultAsync(x => x.Id == request.Id, ct);
        if (entity == null)
        {
            return 0;
        }

        Context.Remove(entity);
        var result = await Context.SaveChangesAsync(ct);
        request.Entity = entity;
        return result;
    }
}