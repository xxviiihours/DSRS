using DSRS.Domain.Inventories;
using System;

namespace DSRS.Application.Contracts;

public interface IDistributionRepository
{
    Task CreateAsync(Inventory inventory);

    Task CreateBatchAsync(List<Inventory> inventories);
}
