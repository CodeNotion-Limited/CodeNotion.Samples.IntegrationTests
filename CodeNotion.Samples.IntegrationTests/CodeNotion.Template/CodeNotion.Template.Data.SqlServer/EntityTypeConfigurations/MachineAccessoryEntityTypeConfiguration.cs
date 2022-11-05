using CodeNotion.Template.Data.SqlServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

#nullable disable

namespace CodeNotion.Template.Data.SqlServer.EntityTypeConfigurations;

internal sealed class MachineAccessoryEntityTypeConfiguration : IEntityTypeConfiguration<MachineAccessory>
{
    public void Configure(EntityTypeBuilder<MachineAccessory> builder)
    {
        builder.ToTable("MachineAccessories");

        builder
            .HasOne(x => x.Machine)
            .WithMany(x => x.MachineAccessories)
            .HasForeignKey(x => x.MachineId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.Accessory)
            .WithMany(x => x.MachineAccessories)
            .HasForeignKey(x => x.AccessoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasIndex(x => new { x.AccessoryId, x.MachineId })
            .IsUnique();
    }
}