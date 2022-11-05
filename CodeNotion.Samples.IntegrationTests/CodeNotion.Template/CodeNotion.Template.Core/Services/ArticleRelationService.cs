using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CodeNotion.Template.Business.Enums;
using CodeNotion.Template.Business.Exceptions;
using CodeNotion.Template.Business.Extensions;
using CodeNotion.Template.Data.SqlServer.Enums;
using CodeNotion.Template.Data.SqlServer.Models;
using CodeNotion.Template.Data.SqlServer.Persistence;

namespace CodeNotion.Template.Business.Services;

internal class ArticleRelationService
{
    private readonly CodeNotionTemplateContext _context;

    public ArticleRelationService(CodeNotionTemplateContext context)
    {
        _context = context;
    }

    internal List<Expression<Func<Article, bool>>> ComposeParameterFilter(string value)
    {
        if (!value.Contains('(') && !value.Contains(')'))
        {
            return new List<Expression<Func<Article, bool>>> {x => x.Code!.StartsWith(value)};
        }

        var match = new Regex(@"\((.*?)\)").Match(value);
        if (!match.Success || match.Groups.Count < 2)
        {
            throw new BadRequestException(ErrorType.InvalidBomFilter);
        }

        var parameterParts = match.Groups[1].Value;
        var articleCodePart = value.Replace(match.Groups[0].Value, "").Trim();
        var keyValueParts = parameterParts.Split(',');
        var result = new List<Expression<Func<Article, bool>>>();
        foreach (var p in keyValueParts)
        {
            var filtersData = p.Split(":");
            if (filtersData.Length < 2)
            {
                result.Add(x => x.Code!.StartsWith(articleCodePart));
                continue;
            }

            var parameterPart = filtersData[0];
            var valuePart = filtersData[1];

            if (!Enum.TryParse<ArticleParameterEnum>(parameterPart, true, out var paramEnum))
            {
                result.Add(x => x.Code!.StartsWith(articleCodePart));
                continue;
            }

            result.Add(x => x.Code!.StartsWith(articleCodePart) && x.ArticleParameters!.Any(d => d.Parameter == paramEnum && d.Value!.Contains(valuePart)));
        }

        return result;
    }

    internal async Task<List<ArticleRelation>> ApplyOperations(int machineVariantId, List<string> additions, List<string> subtractions, CancellationToken ct)
    {
        var inputArticles = await _context
            .Set<Article>()
            .Where(additions.SelectMany(ComposeParameterFilter).ToArray().Or())
            .Include(x => x.ArticleParentRelations!.Where(m => m.MachineVariantArticleRelations!.Any(mv => mv.MachineVariantId == machineVariantId)))
            .ToListAsync(ct);

        var items = new List<ArticleRelation>();
        items.AddRange(inputArticles.SelectMany(x => x.ArticleParentRelations!));
        if (items.Count == 0)
        {
            return items;
        }

        var result = items;
        while (true)
        {
            ct.ThrowIfCancellationRequested();
            var ids = items.Select(x => x.ArticleChildId);

            var childQuery = _context
                .Set<ArticleRelation>()
                .Where(x => x.MachineVariantArticleRelations!.Any(mv => mv.MachineVariantId == machineVariantId))
                .Where(x => ids.Contains(x.ArticleParentId!.Value));

            //ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var subtraction in subtractions)
            {
                childQuery = childQuery.Where(x => !x.ArticleChild!.Code!.StartsWith(subtraction));
            }

            var children = await childQuery.ToListAsync(ct);
            if (children.Count == 0)
            {
                return result;
            }

            result.AddRange(children);
            items = children;
        }
    }
}