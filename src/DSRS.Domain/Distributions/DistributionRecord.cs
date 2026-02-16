using DSRS.Domain.Common;
using DSRS.SharedKernel.Abstractions;
using DSRS.SharedKernel.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSRS.Domain.Distributions;

public class DistributionRecord : EntityBase<Guid>, IAuditableEntity
{
    private DistributionRecord(Guid inventoryId, decimal priceTotal, DistributionType type)
    {
        InventoryId = inventoryId;
        PriceTotal = priceTotal;
        Type = type;
    }

    public DateTime CreatedAt { get; private set; }

    public DateTime LastModified { get; private set; }

    public Guid InventoryId { get; }

    public decimal PriceTotal { get; }

    public DistributionType Type { get; }

    public static Result<DistributionRecord> Create(Guid inventoryId, decimal priceTotal, DistributionType type)
    {
        if (inventoryId == Guid.Empty)
            return Result<DistributionRecord>.Failure(
                new Error("DistributionRecord.InventoryId.Null", "InventoryId cannot be empty."));

        if (priceTotal <= 0)
            return Result<DistributionRecord>.Failure(
                new Error("DistributionRecord.PriceTotal.Invalid", "PriceTotal must be greater than zero."));

        return Result<DistributionRecord>.Success(new DistributionRecord(inventoryId, priceTotal, type));
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
