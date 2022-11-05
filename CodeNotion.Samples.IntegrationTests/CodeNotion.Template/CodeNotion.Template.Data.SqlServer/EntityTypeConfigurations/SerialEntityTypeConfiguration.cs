using CodeNotion.Template.Data.SqlServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

#nullable disable

namespace CodeNotion.Template.Data.SqlServer.EntityTypeConfigurations;

internal sealed class SerialEntityTypeConfiguration : IEntityTypeConfiguration<Serial>
{
    public void Configure(EntityTypeBuilder<Serial> builder)
    {
        builder.ToTable("Serials");

        builder
            .HasOne(x => x.MachineVariant)
            .WithMany(x => x.Serials)
            .HasForeignKey(x => x.MachineVariantId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}