using DSRS.Domain.Inventories;
using System;

namespace DSRS.Application.Contracts;

public interface IDistributionRepository
{
    Task CreateAsync(Inventory inventory);

    Task CreateBatchAsync(List<Inventory> inventories);

    Task UpdateAsync(Inventory inventory);
    Task UpdateBatchAsync(List<Inventory> inventories);

    Task<Inventory> GetById(Guid Id);
}
