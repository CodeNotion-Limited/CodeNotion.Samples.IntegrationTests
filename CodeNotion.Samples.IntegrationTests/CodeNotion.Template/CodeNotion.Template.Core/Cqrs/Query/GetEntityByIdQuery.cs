using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using CodeNotion.Template.Business.Cqrs.Query.Joins;
using CodeNotion.Template.Data.SqlServer.Models.Interfaces;
using CodeNotion.Template.Data.SqlServer.Persistence;

namespace CodeNotion.Template.Business.Cqrs.Query;

public record GetEntityByIdQuery<TEntity> : IRequest<TEntity>, IJoinableQuery
    where TEntity : class, IEntity
{
    public readonly int Id;

    public GetEntityByIdQuery(int id)
    {
        Id = id;
    }

    public Enum[]? QueryJoins { get; init; }
}

internal class GetEntityByIdQueryHandler<TEntity> : IRequestHandler<GetEntityByIdQuery<TEntity>, TEntity?>
    where TEntity : class, IEntity
{
    private readonly FullCodeNotionTemplateContext _context;

    public GetEntityByIdQueryHandler(FullCodeNotionTemplateContext context)
    {
        _context = context;
    }

    public virtual Task<TEntity?> Handle(GetEntityByIdQuery<TEntity> request, CancellationToken ct) => _context
        .Set<TEntity>()
        .IncludeJoins(request.QueryJoins)
        .SingleOrDefaultAsync(x => x.Id == request.Id, ct);
}