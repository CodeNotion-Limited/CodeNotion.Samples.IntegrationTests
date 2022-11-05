using System.Threading;
using System.Threading.Tasks;
using MediatR;
using CodeNotion.Template.Data.SqlServer.Persistence;
using CodeNotion.Odata;

namespace CodeNotion.Template.Business.Cqrs.Query;

// keep this sealed
public sealed record GetOdataQuery<TEntity> : BaseGetOdataQuery<TEntity>, IRequest<ManagedPageResult<TEntity>>
    where TEntity : class
{
}

// keep this sealed
internal sealed class GetOdataQueryHandler<TEntity> : BaseGetOdataQueryHandler<TEntity>, IRequestHandler<GetOdataQuery<TEntity>, ManagedPageResult<TEntity>>
    where TEntity : class
{
    public GetOdataQueryHandler(CodeNotionTemplateContext context, ODataService service) : base(context, service)
    {
    }

    public Task<ManagedPageResult<TEntity>> Handle(GetOdataQuery<TEntity> request, CancellationToken ct) =>
        base.Handle(request, ct);
}