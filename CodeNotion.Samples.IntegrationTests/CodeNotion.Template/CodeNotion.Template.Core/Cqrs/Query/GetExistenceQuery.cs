using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using CodeNotion.Template.Data.SqlServer.Models.Interfaces;
using CodeNotion.Template.Data.SqlServer.Persistence;

namespace CodeNotion.Template.Business.Cqrs.Query;

public class GetExistenceQuery<TEntity> : IRequest<bool>
    where TEntity : class, IEntity
{
    [JsonIgnore] public ODataQueryOptions<TEntity>? Options { get; init; }
}

internal class GetExistenceQueryHandler<TEntity> : IRequestHandler<GetExistenceQuery<TEntity>, bool>
    where TEntity : class, IEntity
{
    private readonly CodeNotionTemplateContext _context;
    private readonly ODataQuerySettings _settings;

    public GetExistenceQueryHandler(CodeNotionTemplateContext context, ODataQuerySettings settings)
    {
        _context = context;
        _settings = settings;
    }

    public virtual Task<bool> Handle(GetExistenceQuery<TEntity> request, CancellationToken ct) =>
        ((IQueryable<TEntity>) request.Options!.Filter.ApplyTo(_context.Set<TEntity>(), _settings)).AnyAsync(ct);
}