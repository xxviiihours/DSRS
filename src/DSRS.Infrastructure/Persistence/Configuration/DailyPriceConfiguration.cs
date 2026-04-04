using DSRS.Domain.Aggregates.Players;
using DSRS.Domain.Aggregates.Pricing;
using DSRS.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DSRS.Infrastructure.Persistence.Configuration;

public class DailyPriceConfiguration : IEntityTypeConfiguration<DailyPrice>
{
    public void Configure(EntityTypeBuilder<DailyPrice> builder)
    {
        builder.HasKey(dp => dp.Id);
        builder.Property(dp => dp.Id)
            .HasDailyPriceIdConversion()
            .ValueGeneratedNever();

        builder.Property(dp => dp.PlayerId)
            .HasPlayerIdConversion()
            .IsRequired();

        builder.Property(dp => dp.ItemId)
            .HasItemIdConversion()
            .IsRequired();

        builder.Property(dp => dp.Price)
            .HasMoneyConversion()
            .IsRequired();

        builder.Property(dp => dp.State)
            .HasConversion<string>()
            .HasMaxLength(4)
            .IsRequired();

        builder.Property(dp => dp.Date)
               .IsRequired();

        builder.HasOne<Player>()
            .WithMany(p => p.DailyPrices)
            .HasForeignKey(dp => dp.PlayerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(dp => dp.Item)
            .WithMany()
            .HasForeignKey(dp => dp.ItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(dp => new
        {
            dp.PlayerId,
            dp.ItemId,
            dp.Date
        }).IsUnique();
    }
}
