using CodeNotion.Template.Data.SqlServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

#nullable disable

namespace CodeNotion.Template.Data.SqlServer.EntityTypeConfigurations;

internal sealed class OrderArticleEntityTypeConfiguration : IEntityTypeConfiguration<OrderArticleRelation>
{
    public void Configure(EntityTypeBuilder<OrderArticleRelation> builder)
    {
        builder.ToTable("OrderArticleRelations");

        builder
            .HasOne(x => x.Order)
            .WithMany(x => x.OrderArticlesRelations)
            .HasForeignKey(x => x.OrderId);

        builder
            .HasOne(x => x.ArticleRelation)
            .WithMany(x => x.OrderArticles)
            .HasForeignKey(x => x.ArticleRelationId);
    }
}