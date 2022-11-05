using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using CodeNotion.Template.Business.Enums;
using CodeNotion.Template.Business.Exceptions;
using CodeNotion.Template.Business.Extensions;
using CodeNotion.Template.Business.Services;
using CodeNotion.Template.Data.SqlServer.Models;
using CodeNotion.Template.Data.SqlServer.Persistence;
using CodeNotion.Odata;

namespace CodeNotion.Template.Business.Cqrs.Query;

public record GetArticleRelationQuery : BaseGetOdataQuery<ArticleRelation>, IRequest<ManagedPageResult<ArticleRelation>>
{
    public int? OrderId { get; init; }
    public int? MeshGroupId { get; init; }
    public int? MachineAccessoryId { get; init; }
    public int? SerialId { get; set; }
    public int? Depth { get; init; }
    public int? MachineVariantId { get; set; }
    public string? Rule { get; set; }
}

internal class GetArticleRelationQueryHandler : BaseGetOdataQueryHandler<ArticleRelation>, IRequestHandler<GetArticleRelationQuery, ManagedPageResult<ArticleRelation>>
{
    private readonly ArticleRelationService _articleRelationService;

    public GetArticleRelationQueryHandler(CodeNotionTemplateContext context, ODataService service, ArticleRelationService articleRelationService) : base(context, service)
    {
        _articleRelationService = articleRelationService;
    }

    public virtual async Task<ManagedPageResult<ArticleRelation>> Handle(GetArticleRelationQuery request, CancellationToken ct)
    {
        var query = Context.Set<ArticleRelation>().AsQueryable();
        if (request.OrderId is not null)
        {
            query = query.Where(x => x.OrderArticles!.Any(or => or.OrderId == request.OrderId));
            return await ToPagedResult(query, request);
        }

        if (request.SerialId is not null)
        {
            query = query.Where(x => x.MachineVariantArticleRelations!.Any(mv => mv.MachineVariant!.Serials!.Any(s => s.Id == request.SerialId)));
        }

        if (request.Depth is null)
        {
            return await ToPagedResult(query, request);
        }

        if (request.SerialId is null && request.MachineVariantId is null)
        {
            throw new BadRequestException(ErrorType.SerialOrMachineVariantRequired, "Serial or MachineVariantId is required when Depth is specified");
        }

        if (request.SerialId is not null && request.MachineVariantId is null)
        {
            request.MachineVariantId = await Context
                .Set<Serial>()
                .Where(x => x.Id == request.SerialId)
                .Select(x => x.MachineVariantId)
                .FirstOrDefaultAsync(ct);

            if (request.MachineVariantId is null)
            {
                throw new BadRequestException(ErrorType.MissingInput, "Missing Machine Variant");
            }
        }

        if (request.Rule is not null)
        {
            return await ToPagedResult(await FilterByRule(request.Rule, request.MachineVariantId!.Value, ct).Then(x => x.AsQueryable()), request);
        }

        if (request.MeshGroupId is null && request.MachineAccessoryId is null)
        {
            return await ToPagedResult(query, request);
        }

        return await ToPagedResult(await FilterByRule(request, request.MachineVariantId!.Value, ct).Then(x => x.AsQueryable()), request);
    }

    private async Task<List<ArticleRelation>> FilterByRule(GetArticleRelationQuery request, int machineVariantId, CancellationToken ct)
    {
        if (request.MeshGroupId is not null && request.MachineAccessoryId is not null)
        {
            throw new BadRequestException(ErrorType.BothMeshGroupIdAndMachineAccessoryIdAreNotAllowed);
        }

        var rule = "";
        if (request.MeshGroupId is not null)
        {
            rule = await Context
                .Set<MeshGroup>()
                .Where(x => x.Id == request.MeshGroupId)
                .Select(x => x.Rules)
                .FirstOrDefaultAsync(ct);
        }

        if (request.MachineAccessoryId is not null)
        {
            rule = await Context
                .Set<MachineAccessory>()
                .Where(x => x.AccessoryId == request.MachineAccessoryId)
                .Select(x => x.Rules)
                .FirstOrDefaultAsync(ct);
        }

        return await FilterByRule(rule, machineVariantId, ct);
    }

    private async Task<List<ArticleRelation>> FilterByRule(string? rule, int machineVariantId, CancellationToken ct)
    {
        if (rule is null)
        {
            return new List<ArticleRelation>(); // no rules yield no results
        }

        var additions = new List<string>();
        var subtractions = new List<string>();
        var outer = rule.Split('+');
        foreach (var group in outer)
        {
            var inner = group.Split("-").Select(x => x.Trim()).ToArray();
            additions.Add(inner[0]);
            subtractions.AddRange(inner.Skip(1));
        }

        return await _articleRelationService.ApplyOperations(machineVariantId, additions, subtractions, ct);
    }
}