using CodeNotion.Template.Data.SqlServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

#nullable disable

namespace CodeNotion.Template.Data.SqlServer.EntityTypeConfigurations;

internal sealed class OrderAccessoryEntityTypeConfiguration : IEntityTypeConfiguration<OrderAccessory>
{
    public void Configure(EntityTypeBuilder<OrderAccessory> builder)
    {
        builder.ToTable("OrderAccessories");

        builder
            .HasOne(x => x.Order)
            .WithMany(x => x.OrderAccessories)
            .HasForeignKey(x => x.OrderId);

        builder
            .HasOne(x => x.Accessory)
            .WithMany(x => x.OrderAccessories)
            .HasForeignKey(x => x.AccessoryId);
    }
}