using DSRS.Domain.Aggregates.Items;

namespace DSRS.Application.Contracts;

public interface IItemRepository 
{
    Task<bool> NameExists(string name);
    Task CreateAsync(Item item);
    Task<List<Item>> GetAllAsync();
}
