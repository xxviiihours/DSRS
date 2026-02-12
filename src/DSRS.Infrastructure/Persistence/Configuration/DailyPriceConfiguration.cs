using DSRS.Domain.Pricing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
            .HasMaxLength(4)
            .IsRequired();

        builder.Property(dp => dp.Date)
               .IsRequired();

        // builder.HasOne(dp => dp.Player)
        //     .WithMany(p => p.DailyPrices)
        //     .HasForeignKey(dp => dp.PlayerId)
        //     .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(dp => dp.Item)
            .WithMany()
            .HasForeignKey(dp => dp.ItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(dp => new
        {
            // dp.PlayerId,
            dp.ItemId,
            dp.Date
        }).IsUnique();
    }
}
