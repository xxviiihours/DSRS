using DSRS.Domain.Aggregates.Inventories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace DSRS.Infrastructure.Persistence.Configuration;

public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.ItemId)
            .IsRequired();
        
        builder.Property(p => p.Quantity)
            .IsRequired();

        builder.Property(p => p.PurchasePrice)
            .IsRequired();

        builder.HasOne(p => p.Item)
            .WithMany()
            .HasForeignKey(p => p.ItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
