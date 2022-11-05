using CodeNotion.Template.Data.SqlServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

#nullable disable

namespace CodeNotion.Template.Data.SqlServer.EntityTypeConfigurations;

internal sealed class ArticleReplacementEntityTypeConfiguration : IEntityTypeConfiguration<ArticleReplacement>
{
    public void Configure(EntityTypeBuilder<ArticleReplacement> builder)
    {
        builder.ToTable("ArticleReplacements");

        builder
            .HasOne(x => x.NewArticle)
            .WithMany(x => x.OldArticleReplacements)
            .OnDelete(DeleteBehavior.NoAction);
            
        builder
            .HasOne(x => x.OldArticle)
            .WithMany(x => x.NewArticleReplacements)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.Machine)
            .WithMany(x => x.ArticleReplacements)
            .OnDelete(DeleteBehavior.Cascade);
    }
}