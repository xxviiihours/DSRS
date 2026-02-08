using DSRS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DSRS.Infrastructure.Persistence.Configuration;

public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.ToTable("Items");
        
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Name)
               .IsRequired()
               .HasMaxLength(100);
        builder.Property(i => i.BasePrice)
               .HasColumnType("decimal(18,2)")
               .IsRequired();
        builder.Property(i => i.Volatility)
               .HasColumnType("decimal(18,2)")
               .IsRequired();
    }
}
