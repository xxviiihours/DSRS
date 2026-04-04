using DSRS.Domain.Aggregates.Inventories;
using DSRS.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DSRS.Infrastructure.Extensions;

public static class VogenPropertyBuilderExtensions
{
    public static PropertyBuilder<PlayerId> HasPlayerIdConversion(
        this PropertyBuilder<PlayerId> builder) =>
            builder.HasConversion(x => x.Value, x => PlayerId.From(x));

    public static PropertyBuilder<ItemId> HasItemIdConversion(
        this PropertyBuilder<ItemId> builder) =>
            builder.HasConversion(x => x.Value, x => ItemId.From(x));

    public static PropertyBuilder<DailyPriceId> HasDailyPriceIdConversion(
        this PropertyBuilder<DailyPriceId> builder) =>
            builder.HasConversion(x => x.Value, x => DailyPriceId.From(x));

    public static PropertyBuilder<InventoryId> HasInventoryIdConversion(
        this PropertyBuilder<InventoryId> builder) =>
            builder.HasConversion(x => x.Value, x => InventoryId.From(x));
    public static PropertyBuilder<DistributionRecordId> HasDistributionRecordIdConversion(
        this PropertyBuilder<DistributionRecordId> builder) =>
            builder.HasConversion(x => x.Value, x => DistributionRecordId.From(x));
    public static PropertyBuilder<PlayerBalanceSnapshotId> HasPlayerBalanceSnapshotIdConversion(
        this PropertyBuilder<PlayerBalanceSnapshotId> builder) =>
            builder.HasConversion(x => x.Value, x => PlayerBalanceSnapshotId.From(x));

    public static PropertyBuilder<FriendshipId> HasFriendshipIdConversion(
        this PropertyBuilder<FriendshipId> builder) =>
            builder.HasConversion(x => x.Value, x => FriendshipId.From(x));
    public static PropertyBuilder<FriendshipPair> HasFriendshipPairConversion(
         this PropertyBuilder<FriendshipPair> builder) =>
            builder.HasConversion(x => x.Value, x => FriendshipPair.From(x));

    public static PropertyBuilder<Money> HasMoneyConversion(
        this PropertyBuilder<Money> builder, int precision = 18, int scale = 2) =>
            builder.HasConversion(x => x.Value, x => Money.From(x))
                .HasPrecision(precision, scale);
}
