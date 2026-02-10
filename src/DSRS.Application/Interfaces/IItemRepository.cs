using DSRS.Domain.Entities;

namespace DSRS.Application.Interfaces;

public interface IItemRepository 
{
    Task<bool> NameExists(string name);
    Task CreateAsync(Item item);
}
