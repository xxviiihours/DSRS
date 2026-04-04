using DSRS.Domain.Aggregates.Players;
using DSRS.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSRS.Infrastructure.Persistence.Configuration;

public class PlayerBalanceSnapshotConfiguration : IEntityTypeConfiguration<PlayerBalanceSnapshot>
{
    public void Configure(EntityTypeBuilder<PlayerBalanceSnapshot> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasPlayerBalanceSnapshotIdConversion()
            .ValueGeneratedNever();

        builder.Property(x => x.PlayerId)
            .HasPlayerIdConversion()
            .IsRequired();

        builder.Property(x => x.Balance)
            .HasMoneyConversion()
            .IsRequired();
    }
}
