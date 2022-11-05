using CodeNotion.Template.Data.SqlServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

#nullable disable

namespace CodeNotion.Template.Data.SqlServer.EntityTypeConfigurations;

internal sealed class MeshGroupEntityTypeConfiguration : IEntityTypeConfiguration<MeshGroup>
{
    public void Configure(EntityTypeBuilder<MeshGroup> builder)
    {
        builder.ToTable("MeshGroups");
    }
}