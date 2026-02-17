using DSRS.Domain.Players;
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
                     .HasColumnType("decimal(18,2)")
                     .IsRequired();

              builder.HasMany(p => p.InventoryItems)
                     .WithOne()
                     .HasForeignKey(i => i.PlayerId)
                     .OnDelete(DeleteBehavior.Cascade);

              builder.HasMany(p => p.DistributionHistory)
                     .WithOne()
                     .HasForeignKey(d => d.PlayerId)
                     .OnDelete(DeleteBehavior.Cascade);
       }
}
