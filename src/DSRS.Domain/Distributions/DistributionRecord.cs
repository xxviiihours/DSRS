using DSRS.Domain.Common;
using DSRS.SharedKernel.Abstractions;
using DSRS.SharedKernel.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSRS.Domain.Distributions;

public class DistributionRecord : EntityBase<Guid>, IAuditableEntity
{
    private DistributionRecord() { }
    internal DistributionRecord(Guid dailyPriceId, Guid playerId, decimal priceTotal, DistributionType type)
    {
        DailyPriceId = dailyPriceId;
        PlayerId = playerId;
        PriceTotal = priceTotal;
        Type = type;
    }

    public DateTime CreatedAt { get; private set; }

    public DateTime LastModified { get; private set; }

    public Guid DailyPriceId { get; }
    public Guid PlayerId { get; set; }

    public decimal PriceTotal { get; }

    public DistributionType Type { get; }

    public static Result<DistributionRecord> Create(Guid dailyPriceId, Guid playerId, decimal priceTotal, DistributionType type)
    {
        if (dailyPriceId == Guid.Empty)
            return Result<DistributionRecord>.Failure(
                new Error("DistributionRecord.DailyPriceId.Null", "DailyPriceId cannot be empty."));

        if (playerId == Guid.Empty)
            return Result<DistributionRecord>.Failure(
                new Error("DistributionRecord.PlayerId.Null", "PlayerId cannot be empty."));

        if (priceTotal <= 0)
            return Result<DistributionRecord>.Failure(
                new Error("DistributionRecord.PriceTotal.Invalid", "PriceTotal must be greater than zero."));

        return Result<DistributionRecord>.Success(new DistributionRecord(dailyPriceId, playerId, priceTotal, type));
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
