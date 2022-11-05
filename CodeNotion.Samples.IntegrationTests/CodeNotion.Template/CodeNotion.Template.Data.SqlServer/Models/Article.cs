using System.Collections.Generic;
using CodeNotion.Template.Data.SqlServer.Enums;
using CodeNotion.Template.Data.SqlServer.Models.Interfaces;

namespace CodeNotion.Template.Data.SqlServer.Models;

public class Article : IEntity
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public ArticleTypeEnum Type { get; set; }
    public string? Description { get; set; }
    public IEnumerable<ArticleRelation>? ArticleParentRelations { get; set; }
    public IEnumerable<ArticleRelation>? ArticleChildRelations { get; set; }
    public IEnumerable<ArticleParameter>? ArticleParameters { get; set; }
    public ICollection<ArticleReplacement>? OldArticleReplacements { get; set; }
    public ICollection<ArticleReplacement>? NewArticleReplacements { get; set; }
}