using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CodeNotion.Template.Data.SqlServer.Models;
using CodeNotion.Template.Data.SqlServer.Persistence;

namespace CodeNotion.Template.Business.Cqrs.Query;

public record GetOrderAvailableMeshesQuery : BaseGetOdataQuery<Mesh>, IRequest<Mesh[]>
{
    public int SerialNumber { get; set; }
}

internal class GetOrderAvailableMeshesQueryHandler : IRequestHandler<GetOrderAvailableMeshesQuery, Mesh[]>
{
    private readonly CodeNotionTemplateContext _context;
    private readonly IServiceProvider _serviceProvider;

    public GetOrderAvailableMeshesQueryHandler(CodeNotionTemplateContext context, IServiceProvider serviceProvider)
    {
        _context = context;
        _serviceProvider = serviceProvider;
    }

    public virtual async Task<Mesh[]> Handle(GetOrderAvailableMeshesQuery request, CancellationToken ct)
    {
        var meshGroups = await _context
            .Set<MeshGroup>()
            .Where(x => !string.IsNullOrWhiteSpace(x.Rules))
            .Where(x => x.Meshes!.Any(m => m.Machine!.MachineVariants!.Any(mv => mv.Serials!.Any(s => s.SerialNumber == request.SerialNumber))))
            .ToArrayAsync(ct);

        var serialId = await _context
            .Set<Serial>()
            .Where(x => x.SerialNumber == request.SerialNumber)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(ct);

        var data = new ConcurrentBag<int>();
        await Parallel.ForEachAsync(meshGroups, new ParallelOptions {MaxDegreeOfParallelism = 10}, async (meshGroup, ctx) =>
        {
            using var scope = _serviceProvider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var relations = await mediator.Send(new GetArticleRelationQuery
            {
                Rule = meshGroup.Rules,
                Depth = 10,
                SerialId = serialId
            }, ctx);

            var a = relations.Items.ToArray();
            if (relations.Items.Any())
            {
                data.Add(meshGroup.Id);
            }
        });

        return await _context
            .Set<Mesh>()
            .Where(x => data.Contains(x.MeshGroupId!.Value))
            .ToArrayAsync(ct);
    }
}