using DSRS.Domain.Entities;

namespace DSRS.Application.Interfaces;

public interface IItemRepository 
{
    Task Create(Item item);
}
