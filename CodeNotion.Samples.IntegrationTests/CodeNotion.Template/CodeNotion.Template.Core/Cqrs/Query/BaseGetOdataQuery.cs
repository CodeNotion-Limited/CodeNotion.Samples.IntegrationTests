using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.OData.Query;
using Newtonsoft.Json;
using CodeNotion.Template.Business.Cqrs.Query.Joins;
using CodeNotion.Template.Data.SqlServer.Persistence;
using CodeNotion.Odata;

namespace CodeNotion.Template.Business.Cqrs.Query;

public abstract record BaseGetOdataQuery<TEntity> : IJoinableQuery
    where TEntity : class
{
    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public ODataQueryOptions<TEntity>? ODataOptions { get; init; }

    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public Enum[]? QueryJoins { get; init; }
}

internal abstract class BaseGetOdataQueryHandler<TEntity>
    where TEntity : class
{
    protected readonly CodeNotionTemplateContext Context;
    protected readonly ODataService Service;

    public BaseGetOdataQueryHandler(CodeNotionTemplateContext context, ODataService service)
    {
        Context = context;
        Service = service;
    }

    public virtual Task<ManagedPageResult<TEntity>> Handle(BaseGetOdataQuery<TEntity> request, CancellationToken ct) =>
        ToPagedResult(Context.Set<TEntity>(), request);

    protected virtual async Task<ManagedPageResult<TEntity>> ToPagedResult(IQueryable<TEntity> query, BaseGetOdataQuery<TEntity> request)
    {
        ManagedPageResult<TEntity> pageResult;
        if (request.ODataOptions == null)
        {
            pageResult = new ManagedPageResult<TEntity>(query.IncludeJoins(request));
        }
        else
        {
            pageResult = await Service.ToPagedResultAsync(query.IncludeJoins(request), request.ODataOptions);
        }

        return pageResult;
    }
}