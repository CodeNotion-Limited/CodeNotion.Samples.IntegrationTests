using CodeNotion.Template.Data.SqlServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

#nullable disable

namespace CodeNotion.Template.Data.SqlServer.EntityTypeConfigurations;

internal sealed class MeshGroupArticleRelationEntityTypeConfiguration : IEntityTypeConfiguration<MeshGroupArticleRelation>
{
    public void Configure(EntityTypeBuilder<MeshGroupArticleRelation> builder)
    {
        builder.ToTable("MeshGroupArticleRelations");

        builder
            .HasOne(x => x.MeshGroup)
            .WithMany(x => x.MeshGroupArticleRelations)
            .HasForeignKey(x => x.MeshGroupId);

        builder
            .HasOne(x => x.ArticleRelation)
            .WithMany(x => x.MeshGroupArticleRelations)
            .HasForeignKey(x => x.ArticleRelationId);

        builder
            .HasOne(x => x.MachineVariant)
            .WithMany(x => x.MeshGroupArticleRelations)
            .HasForeignKey(x => x.MachineVariantId);

        builder
            .HasIndex(x => new {x.MeshGroupId, x.ArticleRelationId, x.MachineVariantId})
            .IsUnique();
    }
}