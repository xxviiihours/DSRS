using DSRS.Domain.Aggregates.Distributions;
using DSRS.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSRS.Infrastructure.Persistence.Configuration;

internal class DistributionConfiguration : IEntityTypeConfiguration<DistributionRecord>
{
    public void Configure(EntityTypeBuilder<DistributionRecord> builder)
    {
        builder.ToTable("DistributionHistory");
        builder.HasKey(dr => dr.Id);
        builder.Property(dr => dr.Id)
            .HasDistributionRecordIdConversion()
            .ValueGeneratedNever();

        builder.Property(dr => dr.DailyPriceId)
            .HasDailyPriceIdConversion()
            .IsRequired();

        builder.Property(dr => dr.PlayerId)
            .HasPlayerIdConversion()
            .IsRequired();

        builder.Property(dr => dr.ItemName)
            .HasMaxLength(25);

        builder.Property(dr => dr.PriceTotal)
            .HasMoneyConversion()
            .IsRequired();

        builder.Property(dr => dr.Type)
            .HasConversion<string>()
            .HasMaxLength(4)
            .IsRequired();

        builder.Property(dr => dr.CreatedAt)
            .IsRequired();

        builder.Property(dr => dr.LastModified)
            .IsRequired();


    }
}
