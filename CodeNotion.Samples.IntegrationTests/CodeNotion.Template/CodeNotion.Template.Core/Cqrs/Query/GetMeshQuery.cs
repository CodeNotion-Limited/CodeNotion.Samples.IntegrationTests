using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using CodeNotion.Template.Data.SqlServer.Models;
using CodeNotion.Template.Data.SqlServer.Persistence;
using CodeNotion.Odata;

namespace CodeNotion.Template.Business.Cqrs.Query;

public record GetMeshQuery : BaseGetOdataQuery<Mesh>, IRequest<ManagedPageResult<Mesh>>
{
    public int? OrderId { get; init; }
}

internal class GetMeshQueryHandler : BaseGetOdataQueryHandler<Mesh>, IRequestHandler<GetMeshQuery, ManagedPageResult<Mesh>>
{
    public GetMeshQueryHandler(FullCodeNotionTemplateContext context, ODataService service) : base(context, service)
    {
    }

    public virtual async Task<ManagedPageResult<Mesh>> Handle(GetMeshQuery request, CancellationToken ct)
    {
        var query = Context.Set<Mesh>().AsQueryable();

        if (request.OrderId is not null)
        {
            query = query.Where(x => x.OrderMeshes!.Any(o => o.OrderId == request.OrderId));
        }

        return await ToPagedResult(query, request);
    }
}