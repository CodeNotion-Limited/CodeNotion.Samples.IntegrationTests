using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using CodeNotion.Template.Data.SqlServer.Models;
using CodeNotion.Template.Data.SqlServer.Persistence;
using CodeNotion.Odata;

namespace CodeNotion.Template.Business.Cqrs.Query;

public record GetAccessoryQuery : BaseGetOdataQuery<Accessory>, IRequest<ManagedPageResult<Accessory>>
{
    public int? OrderId { get; set; }
    public int? MachineId { get; set; }
    public int? IncludeMachineAccessoryForMachineId { get; set; }
}

internal class GetAccessoryQueryHandler : BaseGetOdataQueryHandler<Accessory>, IRequestHandler<GetAccessoryQuery, ManagedPageResult<Accessory>>
{
    public GetAccessoryQueryHandler(CodeNotionTemplateContext context, ODataService service) : base(context, service)
    {
    }

    public virtual Task<ManagedPageResult<Accessory>> Handle(GetAccessoryQuery request, CancellationToken ct)
    {
        var query = Context.Set<Accessory>().AsQueryable();
        if (request.IncludeMachineAccessoryForMachineId is not null)
        {
            query = query.Include(a => a.MachineAccessories!.Where(x => x.MachineId == request.IncludeMachineAccessoryForMachineId));
        }

        if (request.MachineId is not null)
        {
            query = query.Where(x => x.MachineAccessories!.Any(m => m.MachineId == request.MachineId));
        }

        if (request.OrderId is not null)
        {
            query = query.Where(x => x.OrderAccessories!.Any(o => o.OrderId == request.OrderId));
        }

        return ToPagedResult(query, request);
    }
}