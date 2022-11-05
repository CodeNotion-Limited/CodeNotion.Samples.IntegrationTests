using CodeNotion.Template.Data.SqlServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

#nullable disable
namespace CodeNotion.Template.Data.SqlServer.EntityTypeConfigurations;

internal sealed class MachineVariantEntityTypeConfiguration : IEntityTypeConfiguration<MachineVariant>
{
    public void Configure(EntityTypeBuilder<MachineVariant> builder)
    {
        builder.ToTable("MachineVariants");

        builder
            .HasOne(x => x.Machine)
            .WithMany(x => x.MachineVariants)
            .HasForeignKey(x => x.MachineId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}