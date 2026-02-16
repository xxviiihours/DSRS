using DSRS.Domain.Inventories;

namespace DSRS.Application.Contracts;

public interface IInventoryRepository
{
    Task CreateAsync(Inventory inventory);

    Task CreateBatchAsync(List<Inventory> inventories);

    Task UpdateAsync(Inventory inventory);
    Task UpdateBatchAsync(List<Inventory> inventories);

    Task<Inventory> GetById(Guid Id);
}
