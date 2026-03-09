using DSRS.Domain.Aggregates.Friendships;
using DSRS.Domain.Aggregates.Players;
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

        builder.HasIndex(x => new { x.PlayerA, x.PlayerB })
            .IsUnique();
    }
}
