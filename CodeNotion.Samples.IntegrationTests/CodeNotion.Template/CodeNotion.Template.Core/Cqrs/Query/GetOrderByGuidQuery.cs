using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using CodeNotion.Template.Business.Cqrs.Query.Joins;
using CodeNotion.Template.Data.SqlServer.Models;
using CodeNotion.Template.Data.SqlServer.Persistence;

namespace CodeNotion.Template.Business.Cqrs.Query;

public record GetOrderByGuidQuery : IRequest<Order?>
{
    public Guid Guid { get; init; }
    public Enum[]? Joins { get; init; }
}

internal class GetOrderByGuidQueryHandler : IRequestHandler<GetOrderByGuidQuery, Order?>
{
    private readonly CodeNotionTemplateContext _context;

    public GetOrderByGuidQueryHandler(CodeNotionTemplateContext context)
    {
        _context = context;
    }

    public Task<Order?> Handle(GetOrderByGuidQuery request, CancellationToken ct) =>
        _context
            .Set<Order>()
            .Where(x => x.Guid.Equals(request.Guid))
            .IncludeJoins(request.Joins)
            .FirstOrDefaultAsync(ct);
}