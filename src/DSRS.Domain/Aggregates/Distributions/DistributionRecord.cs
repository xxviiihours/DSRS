using DSRS.Domain.Aggregates.Items;
using DSRS.Domain.Common;
using DSRS.Domain.ValueObjects;
using DSRS.SharedKernel.Abstractions;
using DSRS.SharedKernel.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSRS.Domain.Aggregates.Distributions;

public class DistributionRecord : AggregateRoot<DistributionRecordId>, IAuditableEntity
{
    //private DistributionRecord() { }
    public DateTime CreatedAt { get; private set; }

    public DateTime LastModified { get; private set; }

    public PlayerId PlayerId { get; }
    public DailyPriceId DailyPriceId { get; }
    public string ItemName { get; } = string.Empty;
    public Money PriceTotal { get; }

    public DistributionType Type { get; }

    internal DistributionRecord(
        PlayerId playerId,
        DailyPriceId dailyPriceId, 
        string itemName,
        Money priceTotal, 
        DistributionType type)
    {
        Id = DistributionRecordId.New();
        PlayerId = playerId;
        DailyPriceId = dailyPriceId;
        ItemName = itemName;
        PriceTotal = priceTotal;
        Type = type;
    }

    public static Result<DistributionRecord> Create(
        PlayerId playerId,
        DailyPriceId dailyPriceId, 
        string itemName,
        Money priceTotal, 
        DistributionType type)
    {
        if (dailyPriceId.IsEmpty())
            return Result<DistributionRecord>.Failure(
                new Error("DistributionRecord.DailyPriceId.Null", "DailyPriceId cannot be empty."));

        if (playerId.IsEmpty())
            return Result<DistributionRecord>.Failure(
                new Error("DistributionRecord.PlayerId.Null", "PlayerId cannot be empty."));

        if (priceTotal.IsZero() || priceTotal.IsNegative())
            return Result<DistributionRecord>.Failure(
                new Error("DistributionRecord.PriceTotal.Invalid", "PriceTotal must be greater than zero."));

        return Result<DistributionRecord>.Success(
            new DistributionRecord(
                playerId,
                dailyPriceId,  
                itemName, 
                priceTotal, 
                type));
    }

    public void SetCreated(DateTime now)
    {
        CreatedAt = now;
        LastModified = now;
    }

    public void SetModified(DateTime now)
    {
        LastModified = now;
    }
}
