using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using CodeNotion.Template.Data.SqlServer.Models;
using CodeNotion.Template.Data.SqlServer.Persistence;
using CodeNotion.Odata;

namespace CodeNotion.Template.Business.Cqrs.Query;

public record GetSerialQuery : BaseGetOdataQuery<Serial>, IRequest<ManagedPageResult<Serial>>
{
    public int? MachineId { get; set; }
}

internal class GetSerialQueryHandler : BaseGetOdataQueryHandler<Serial>, IRequestHandler<GetSerialQuery, ManagedPageResult<Serial>>
{
    public GetSerialQueryHandler(CodeNotionTemplateContext context, ODataService service) : base(context, service)
    {
    }

    public virtual Task<ManagedPageResult<Serial>> Handle(GetSerialQuery request, CancellationToken ct)
    {
        var query = Context.Set<Serial>().AsQueryable();

        if (request.MachineId is not null)
        {
            query = query.Where(x => x.MachineVariant!.MachineId == request.MachineId);
        }

        return ToPagedResult(query, request);
    }
}