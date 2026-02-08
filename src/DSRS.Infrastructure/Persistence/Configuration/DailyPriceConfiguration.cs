using DSRS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSRS.Infrastructure.Persistence.Configuration;

public class DailyPriceConfiguration : IEntityTypeConfiguration<DailyPrice>
{
    public void Configure(EntityTypeBuilder<DailyPrice> builder)
    {
        builder.HasKey(dp => dp.Id);
        builder.Property(dp => dp.Price)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        builder.Property(dp => dp.State)
            .HasConversion<string>()
            .IsRequired();
        builder.HasOne(dp => dp.Player)
            .WithMany()
            .HasForeignKey(dp => dp.PlayerId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(dp => dp.Item)
            .WithMany()
            .HasForeignKey(dp => dp.ItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
