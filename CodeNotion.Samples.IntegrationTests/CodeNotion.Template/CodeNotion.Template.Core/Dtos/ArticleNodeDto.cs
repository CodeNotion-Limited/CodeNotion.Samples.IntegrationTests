using System.Collections.Generic;
using System.Linq;
using CodeNotion.Template.Data.SqlServer.Models;

namespace CodeNotion.Template.Business.Dtos;

public record ArticleNodeDto(Article Item)
{
    public IEnumerable<ArticleNodeDto>? Children { get; set; }
        
    public ArticleNodeDto CleanReferences()
    {
        Item.ArticleParentRelations = null;
        Item.ArticleChildRelations = null;

        Children = Children?.Select(x => x.CleanReferences());
        return this;
    }
}