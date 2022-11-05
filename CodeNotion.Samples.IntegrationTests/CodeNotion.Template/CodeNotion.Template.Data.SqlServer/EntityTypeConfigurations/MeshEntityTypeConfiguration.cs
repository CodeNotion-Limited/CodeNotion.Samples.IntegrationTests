using CodeNotion.Template.Data.SqlServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

#nullable disable

namespace CodeNotion.Template.Data.SqlServer.EntityTypeConfigurations;

internal sealed class MeshEntityTypeConfiguration : IEntityTypeConfiguration<Mesh>
{
    public void Configure(EntityTypeBuilder<Mesh> builder)
    {
        builder.ToTable("Meshes");

        builder
            .HasOne(x => x.Machine)
            .WithMany(x => x.Meshes)
            .HasForeignKey(x => x.MachineId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.MeshGroup)
            .WithMany(x => x.Meshes)
            .HasForeignKey(x => x.MeshGroupId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}