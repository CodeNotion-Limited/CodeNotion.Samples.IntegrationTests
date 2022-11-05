using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeNotion.Template.Business.Dtos;

public class ArticleRelationNode
{
    public int ItemId { get; set; }
    public IEnumerable<int>? ChildrenIds { get; set; }
    public IEnumerable<ArticleRelationNode>? Children { get; set; }
    public List<int>? RelationIds { get; set; }
    private int[]? RecursiveChildrenIds { get; set; }

    public IEnumerable<int> GetChildrenRelations()
    {
        if (RelationIds is null)
        {
            throw new ArgumentNullException(nameof(RelationIds));
        }

        var result = RelationIds;
        if (Children is null)
        {
            return result;
        }

        RecursiveChildrenIds ??= Children.SelectMany(x => x.GetChildrenRelations()).ToArray();
        return result.Concat(RecursiveChildrenIds).Distinct().ToList();
    }
}