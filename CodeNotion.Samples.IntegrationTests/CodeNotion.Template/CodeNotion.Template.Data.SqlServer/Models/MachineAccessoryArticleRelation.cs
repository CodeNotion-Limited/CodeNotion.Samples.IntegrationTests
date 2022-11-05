using CodeNotion.Template.Data.SqlServer.Models.Interfaces;

namespace CodeNotion.Template.Data.SqlServer.Models;

public class MachineAccessoryArticleRelation : IEntity
{
    public int Id { get; set; }
    public int MachineAccessoryId { get; set; }
    public int ArticleRelationId { get; set; }
    public MachineAccessory? MachineAccessory { get; set; }
    public ArticleRelation? ArticleRelation { get; set; }
}