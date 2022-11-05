using CodeNotion.Template.Data.SqlServer.Models.Interfaces;

namespace CodeNotion.Template.Data.SqlServer.Models;

public class MachineVariantArticleRelation : IEntity
{
    public int Id { get; set; }
    public int MachineVariantId { get; set; }
    public MachineVariant? MachineVariant { get; set; }
    public int ArticleRelationId { get; set; }
    public ArticleRelation? ArticleRelation { get; set; }
}