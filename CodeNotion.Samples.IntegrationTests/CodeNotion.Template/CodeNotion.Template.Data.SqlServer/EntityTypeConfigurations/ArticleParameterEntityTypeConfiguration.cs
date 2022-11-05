using CodeNotion.Template.Data.SqlServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

#nullable disable

namespace CodeNotion.Template.Data.SqlServer.EntityTypeConfigurations;

internal sealed class ArticleParameterEntityTypeConfiguration : IEntityTypeConfiguration<ArticleParameter>
{
    public void Configure(EntityTypeBuilder<ArticleParameter> builder)
    {
        builder.ToTable("ArticleParameters");

        builder
            .HasOne(x => x.Article)
            .WithMany(x => x.ArticleParameters)
            .HasForeignKey(x => x.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}