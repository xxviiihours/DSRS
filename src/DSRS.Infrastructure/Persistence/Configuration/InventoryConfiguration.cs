using DSRS.Domain.Aggregates.Inventories;
using DSRS.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DSRS.Infrastructure.Persistence.Configuration;

public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .HasInventoryIdConversion()
            .ValueGeneratedNever();

        builder.Property(p => p.ItemId)
            .HasItemIdConversion()
            .IsRequired();
        
        builder.Property(p => p.Quantity)
            .IsRequired();

        builder.Property(p => p.PurchasePrice)
            .HasMoneyConversion()
            .IsRequired();

        builder.HasOne(p => p.Item)
            .WithMany()
            .HasForeignKey(p => p.ItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
