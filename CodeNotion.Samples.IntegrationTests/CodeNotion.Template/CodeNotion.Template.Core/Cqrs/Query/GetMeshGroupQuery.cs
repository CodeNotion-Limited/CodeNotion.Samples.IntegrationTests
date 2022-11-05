using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using CodeNotion.Template.Data.SqlServer.Models;
using CodeNotion.Template.Data.SqlServer.Persistence;
using CodeNotion.Odata;

namespace CodeNotion.Template.Business.Cqrs.Query;

public record GetMeshGroupQuery : BaseGetOdataQuery<MeshGroup>, IRequest<ManagedPageResult<MeshGroup>>
{
    public int? MachineId { get; set; }
}

internal class GetMeshGroupQueryHandler : BaseGetOdataQueryHandler<MeshGroup>, IRequestHandler<GetMeshGroupQuery, ManagedPageResult<MeshGroup>>
{
    public GetMeshGroupQueryHandler(FullCodeNotionTemplateContext context, ODataService service) : base(context, service)
    {
    }

    public virtual Task<ManagedPageResult<MeshGroup>> Handle(GetMeshGroupQuery request, CancellationToken ct)
    {
        var query = Context.Set<MeshGroup>().AsQueryable();

        if (request.MachineId is not null)
        {
            query = query.Where(x => x.Meshes!.Any(m => m.MachineId == request.MachineId));
        }

        return ToPagedResult(query, request);
    }
}