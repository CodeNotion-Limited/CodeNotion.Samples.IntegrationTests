using CodeNotion.Template.Data.SqlServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

#nullable disable

namespace CodeNotion.Template.Data.SqlServer.EntityTypeConfigurations;

internal sealed class ArticleRelationEntityTypeConfiguration : IEntityTypeConfiguration<ArticleRelation>
{
    public void Configure(EntityTypeBuilder<ArticleRelation> builder)
    {
        builder.ToTable("ArticleRelations");

        builder.Property(x => x.Quantity).HasPrecision(14, 4);

        builder
            .HasOne(x => x.ArticleChild)
            .WithMany(x => x.ArticleParentRelations)
            .HasForeignKey(x => x.ArticleChildId)
            .OnDelete(DeleteBehavior.ClientCascade); // Cannot have Cascade because of cycles

        builder
            .HasOne(x => x.ArticleParent)
            .WithMany(x => x.ArticleChildRelations)
            .HasForeignKey(x => x.ArticleParentId)
            .OnDelete(DeleteBehavior.ClientCascade); // Cannot have Cascade because of cycles

        builder
            .HasIndex(x => new { x.ArticleParentId, x.ArticleChildId})
            .IsUnique();
    }
}