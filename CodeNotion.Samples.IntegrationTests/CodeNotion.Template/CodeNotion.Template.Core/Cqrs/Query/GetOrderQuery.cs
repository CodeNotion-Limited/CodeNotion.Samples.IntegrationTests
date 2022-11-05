using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using CodeNotion.Template.Data.SqlServer.Models;
using CodeNotion.Template.Data.SqlServer.Persistence;
using CodeNotion.Odata;

namespace CodeNotion.Template.Business.Cqrs.Query;

public record GetOrderQuery : BaseGetOdataQuery<Order>, IRequest<ManagedPageResult<Order>>
{
    public int? MachineId { get; set; }
}

internal class GetOrderQueryHandler : BaseGetOdataQueryHandler<Order>, IRequestHandler<GetOrderQuery, ManagedPageResult<Order>>
{
    public GetOrderQueryHandler(CodeNotionTemplateContext context, ODataService service) : base(context, service)
    {
    }

    public virtual Task<ManagedPageResult<Order>> Handle(GetOrderQuery request, CancellationToken ct)
    {
        var query = Context.Set<Order>().AsQueryable();

        if (request.MachineId is not null)
        {
            query = query.Where(x => x.Serial!.MachineVariant!.MachineId == request.MachineId);
        }

        return ToPagedResult(query, request);
    }
}