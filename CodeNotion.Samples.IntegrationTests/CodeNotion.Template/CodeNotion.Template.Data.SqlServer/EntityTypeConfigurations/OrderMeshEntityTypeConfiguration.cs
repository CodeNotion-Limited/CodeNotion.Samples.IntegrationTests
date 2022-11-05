using CodeNotion.Template.Data.SqlServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

#nullable disable

namespace CodeNotion.Template.Data.SqlServer.EntityTypeConfigurations;

internal sealed class OrderMeshEntityTypeConfiguration : IEntityTypeConfiguration<OrderMesh>
{
    public void Configure(EntityTypeBuilder<OrderMesh> builder)
    {
        builder.ToTable("OrderMeshes");

        builder
            .HasOne(x => x.Order)
            .WithMany(x => x.OrderMeshes)
            .HasForeignKey(x => x.OrderId);

        builder
            .HasOne(x => x.Mesh)
            .WithMany(x => x.OrderMeshes)
            .HasForeignKey(x => x.MeshId);
    }
}