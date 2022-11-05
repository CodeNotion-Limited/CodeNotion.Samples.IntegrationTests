using CodeNotion.Template.Data.SqlServer.Models.Interfaces;

namespace CodeNotion.Template.Data.SqlServer.Models;

public class OrderArticleRelation: IEntity
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ArticleRelationId { get; set; }
    public Order? Order { get; set; }
    public ArticleRelation? ArticleRelation { get; set; }
}