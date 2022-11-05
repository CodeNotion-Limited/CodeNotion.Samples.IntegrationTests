using CodeNotion.Template.Data.SqlServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

#nullable disable

namespace CodeNotion.Template.Data.SqlServer.EntityTypeConfigurations;

internal sealed class MachineAccessoryArticleRelationEntityTypeConfiguration : IEntityTypeConfiguration<MachineAccessoryArticleRelation>
{
    public void Configure(EntityTypeBuilder<MachineAccessoryArticleRelation> builder)
    {
        builder.ToTable("MachineAccessoryArticleRelations");

        builder
            .HasOne(x => x.MachineAccessory)
            .WithMany(x => x.MachineAccessoryArticleRelations)
            .HasForeignKey(x => x.MachineAccessoryId);

        builder
            .HasOne(x => x.ArticleRelation)
            .WithMany(x => x.MachineAccessoryArticleRelations)
            .HasForeignKey(x => x.ArticleRelationId);

        builder
            .HasIndex(x => new { x.MachineAccessoryId, x.ArticleRelationId, })
            .IsUnique();
    }
}