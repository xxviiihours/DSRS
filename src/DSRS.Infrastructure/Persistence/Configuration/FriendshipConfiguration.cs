using DSRS.Domain.Aggregates.Friendships;
using DSRS.Domain.Aggregates.Players;
using DSRS.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSRS.Infrastructure.Persistence.Configuration;

public class FriendshipConfiguration : IEntityTypeConfiguration<Friendship>
{
    public void Configure(EntityTypeBuilder<Friendship> builder)
    {
        builder.ToTable("Friendships");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasFriendshipIdConversion()
            .ValueGeneratedNever();

        builder.Property(x => x.Pair)
            .HasFriendshipPairConversion()
            .IsRequired()
            .HasColumnName("PlayerPair");

        builder.Property(x => x.RequesterId)
            .HasPlayerIdConversion()
            .IsRequired()
            .HasColumnName("RequesterId");

        builder.Property(x => x.AddresseeId)
            .HasPlayerIdConversion()
            .IsRequired()
            .HasColumnName("AddresseeId");

        builder.Property(x => x.Status)
            .HasConversion<int>();

        builder.HasOne(x => x.Requester)
            .WithMany()
            .HasForeignKey(x => x.RequesterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Addressee)
            .WithMany()
            .HasForeignKey(x => x.AddresseeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.Pair)
            .HasDatabaseName("IX_Friendships_UniquePair")
            .IsUnique();

        builder.HasIndex(x => new { x.Pair, x.Status })
            .HasDatabaseName("IX_Friendships_PairStatus");

        builder.HasIndex(x => x.RequesterId)
            .HasDatabaseName("IX_Friendships_Requester");

        builder.HasIndex(x => x.AddresseeId)
            .HasDatabaseName("IX_Friendships_Addressee");
    }
}
