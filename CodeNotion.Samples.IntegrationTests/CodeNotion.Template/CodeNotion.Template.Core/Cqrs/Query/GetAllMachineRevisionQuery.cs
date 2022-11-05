using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using CodeNotion.Template.Business.Cqrs.Query.Joins;
using CodeNotion.Template.Business.Dtos;
using CodeNotion.Template.Data.SqlServer.Models;
using CodeNotion.Template.Data.SqlServer.Persistence;
using CodeNotion.Odata;

namespace CodeNotion.Template.Business.Cqrs.Query;

public record GetAllMachineRevisionQuery : IRequest<ICollection<MachineRevisionGroupDto>>, IJoinableQuery
{
    public Enum[]? QueryJoins { get; init; }
}

internal class GetAllMachineRevisionQueryHandler : IRequestHandler<GetAllMachineRevisionQuery, ICollection<MachineRevisionGroupDto>>
{
    private readonly FullCodeNotionTemplateContext _context;

    public GetAllMachineRevisionQueryHandler(FullCodeNotionTemplateContext context, ODataService service)
    {
        _context = context;
    }

    public async Task<ICollection<MachineRevisionGroupDto>> Handle(GetAllMachineRevisionQuery request, CancellationToken ct)
    {
        var query = await _context
            .Set<Machine>()
            .ToListAsync(ct);

        var group = query
            .GroupBy(machine => machine.Name)
            .OrderBy(x => x.Key);

        var result = new List<MachineRevisionGroupDto>();
        foreach (var grouping in group)
        {
            var i = 1;
            result.Add(new MachineRevisionGroupDto()
            {
                Name = grouping.Key,
                Children = grouping.Select(machine => new MachineRevisionDto()
                {
                    Machine = machine,
                    RevisionNumber = i++
                }).ToList()
            });
        }

        return result;
    }
}