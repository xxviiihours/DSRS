using DSRS.Domain.Items;

namespace DSRS.Application.Interfaces;

public interface IItemRepository 
{
    Task<bool> NameExists(string name);
    Task CreateAsync(Item item);
    Task<List<Item>> GetAllAsync();
}
