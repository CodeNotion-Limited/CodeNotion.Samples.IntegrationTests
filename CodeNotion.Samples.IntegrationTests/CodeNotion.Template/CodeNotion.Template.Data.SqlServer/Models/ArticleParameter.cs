using CodeNotion.Template.Data.SqlServer.Enums;
using CodeNotion.Template.Data.SqlServer.Models.Interfaces;

namespace CodeNotion.Template.Data.SqlServer.Models;

public class ArticleParameter : IEntity
{
    public int Id { get; set; }
    public int ArticleId { get; set; }
    public Article? Article { get; set; }
    public string? Group { get; set; }
    public ArticleParameterEnum? Parameter { get; set; }
    public string? Value { get; set; }
}