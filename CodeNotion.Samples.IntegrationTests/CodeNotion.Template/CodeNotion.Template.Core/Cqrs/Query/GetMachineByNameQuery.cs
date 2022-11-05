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

public record GetMachineByNameQuery(string Name) : IRequest<Machine?>, IJoinableQuery
{
    public Enum[]? QueryJoins { get; init; }
}

internal class GetMachineByNameQueryHandler : IRequestHandler<GetMachineByNameQuery, Machine?>
{
    private readonly FullCodeNotionTemplateContext _context;

    public GetMachineByNameQueryHandler(FullCodeNotionTemplateContext context)
    {
        _context = context;
    }

    public Task<Machine?> Handle(GetMachineByNameQuery request, CancellationToken ct) => _context
        .Set<Machine>()
        .Where(x => x.Name == request.Name)
        .IncludeJoins(request.QueryJoins)
        .FirstOrDefaultAsync(ct)!;
}