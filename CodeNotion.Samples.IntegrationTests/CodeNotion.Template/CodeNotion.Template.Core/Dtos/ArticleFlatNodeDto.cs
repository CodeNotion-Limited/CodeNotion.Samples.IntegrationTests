using CodeNotion.Template.Data.SqlServer.Models;

namespace CodeNotion.Template.Business.Dtos;

public record ArticleFlatNodeDto(Article Item, ArticleRelation? Relation, int Depth)
{
}