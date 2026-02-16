using DSRS.Domain.Inventories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace DSRS.Infrastructure.Persistence.Configuration;

public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> builder)
    {
        builder.HasKey(dp => dp.Id);

        builder.Property(dp => dp.ItemId)
            .IsRequired();
        
        builder.Property(dp => dp.Quantity)
            .IsRequired();

        builder.HasOne(dp => dp.Item)
            .WithMany()
            .HasForeignKey(dp => dp.ItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
