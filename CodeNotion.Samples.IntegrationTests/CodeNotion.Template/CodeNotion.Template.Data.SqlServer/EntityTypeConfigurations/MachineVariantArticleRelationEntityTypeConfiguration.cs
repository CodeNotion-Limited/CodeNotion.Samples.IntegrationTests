using CodeNotion.Template.Data.SqlServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

#nullable disable

namespace CodeNotion.Template.Data.SqlServer.EntityTypeConfigurations;

internal sealed class MachineVariantArticleRelationEntityTypeConfiguration : IEntityTypeConfiguration<MachineVariantArticleRelation>
{
    public void Configure(EntityTypeBuilder<MachineVariantArticleRelation> builder)
    {
        builder.ToTable("MachineVariantArticleRelations");

        builder
            .HasOne(x => x.MachineVariant)
            .WithMany(x => x.MachineVariantArticleRelations)
            .HasForeignKey(x => x.MachineVariantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.ArticleRelation)
            .WithMany(x => x.MachineVariantArticleRelations)
            .HasForeignKey(x => x.ArticleRelationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasIndex(x => new { x.MachineVariantId, x.ArticleRelationId })
            .IsUnique();
    }
}