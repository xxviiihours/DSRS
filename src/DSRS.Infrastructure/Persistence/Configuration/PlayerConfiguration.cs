using DSRS.Domain.Aggregates.Players;
using DSRS.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DSRS.Infrastructure.Persistence.Configuration;

public class PlayerConfiguration : IEntityTypeConfiguration<Player>
{
    public void Configure(EntityTypeBuilder<Player> builder)
    {
        builder.ToTable("Players");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Balance)
            .HasMoneyConversion(precision: 18, scale: 2)
            .IsRequired();


        builder.Property(p => p.PurchaseLimit)
            .IsRequired();

        builder.Property(p => p.LastLimitGeneration)
            .IsRequired();

        builder.HasMany(p => p.InventoryItems)
            .WithOne()
            .HasForeignKey(i => i.PlayerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
