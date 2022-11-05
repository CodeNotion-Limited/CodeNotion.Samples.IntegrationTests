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

public record GetMachineBySerialQuery(int Serial) : IRequest<Machine?>, IJoinableQuery
{
    public Enum[]? QueryJoins { get; init; }
}

internal class GetMachineBySerialQueryHandler : IRequestHandler<GetMachineBySerialQuery, Machine?>
{
    private readonly FullCodeNotionTemplateContext _context;

    public GetMachineBySerialQueryHandler(FullCodeNotionTemplateContext context)
    {
        _context = context;
    }

    public Task<Machine?> Handle(GetMachineBySerialQuery request, CancellationToken ct) => _context
        .Set<Machine>()
        .Where(x => x.MachineVariants!.Any(mv => mv.Serials!.Any(s => s.SerialNumber == request.Serial)))
        .IncludeJoins(request.QueryJoins)
        .FirstOrDefaultAsync(ct)!;
}