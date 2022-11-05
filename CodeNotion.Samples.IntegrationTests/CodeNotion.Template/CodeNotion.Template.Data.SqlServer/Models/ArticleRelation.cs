using System.Collections.Generic;
using CodeNotion.Template.Data.SqlServer.Models.Interfaces;

namespace CodeNotion.Template.Data.SqlServer.Models;

public class ArticleRelation : IEntity
{
    public int Id { get; set; }
    public int? ArticleParentId { get; set; }
    public Article? ArticleParent { get; set; }
    public int ArticleChildId { get; set; }
    public Article? ArticleChild { get; set; }
    public decimal Quantity { get; set; }
    public ICollection<OrderArticleRelation>? OrderArticles { get; set; }
    public ICollection<MachineAccessoryArticleRelation>? MachineAccessoryArticleRelations { get; set; }
    public ICollection<MeshGroupArticleRelation>? MeshGroupArticleRelations { get; set; }
    public ICollection<MachineVariantArticleRelation>? MachineVariantArticleRelations { get; set; }
}