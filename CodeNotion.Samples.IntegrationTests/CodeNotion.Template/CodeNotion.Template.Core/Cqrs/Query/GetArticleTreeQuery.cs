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

namespace CodeNotion.Template.Business.Cqrs.Query;

public record GetArticleFlatTreeQuery : IRequest<List<ArticleFlatNodeDto>>, IJoinableQuery
{
    public int Serial { get; init; }
    public int Depth { get; init; }
    public Enum[]? QueryJoins { get; init; }
}

internal class GetArticleTreeQueryHandler : IRequestHandler<GetArticleFlatTreeQuery, List<ArticleFlatNodeDto>>
{
    protected readonly FullCodeNotionTemplateContext Context;

    public GetArticleTreeQueryHandler(FullCodeNotionTemplateContext context)
    {
        Context = context;
    }

    public async Task<List<ArticleFlatNodeDto>> Handle(GetArticleFlatTreeQuery request, CancellationToken ct)
    {
        var relations = await Context
            .Set<ArticleRelation>()
            .Where(x => x.MachineVariantArticleRelations!.Any(mv => mv.MachineVariant!.Serials!.Any(s => s.SerialNumber == request.Serial)))
            .Include(x => x.ArticleChild)
            .Include(x => x.ArticleParent)
            .IncludeJoins(request)
            .ToArrayAsync(ct);

        var root = relations.Where(x => x.ArticleParent?.Code?.StartsWith("ARG") ?? false).ToArray();
        var result = new List<ArticleFlatNodeDto>();
        var stack = new Stack<(ArticleRelation, int)>(root.Select(x => (x, 0)));

        while (stack.Count > 0)
        {
            var (relation, depth) = stack.Pop();
            result.Add(new ArticleFlatNodeDto(relation.ArticleChild!, relation, depth));
            if (depth >= request.Depth)
            {
                continue;
            }

            foreach (var child in relations.Where(x => x.ArticleParentId == relation.ArticleChildId))
            {
                stack.Push((child, depth + 1));
            }
        }

        return result.Select(x =>
        {
            x.Relation!.ArticleParent = null;
            if (x.Relation!.ArticleChild == null)
            {
                return x;
            }

            x.Relation!.ArticleChild.ArticleChildRelations = null;
            x.Relation!.ArticleChild.ArticleParentRelations = null;
            x.Relation!.ArticleChild = null;
            return x;
        }).ToList();
    }
}