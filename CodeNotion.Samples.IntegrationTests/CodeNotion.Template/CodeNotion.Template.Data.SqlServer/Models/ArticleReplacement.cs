using CodeNotion.Template.Data.SqlServer.Models.Interfaces;

namespace CodeNotion.Template.Data.SqlServer.Models;

public class ArticleReplacement : IEntity
{
    public int Id { get; set; }
    public int OldArticleId { get; set; }
    public Article? OldArticle { get; set; }
    public int NewArticleId { get; set; }
    public Article? NewArticle { get; set; }
    public int? MachineId { get; set; }
    public Machine? Machine { get; set; }
}