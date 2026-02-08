using DSRS.Domain.Entities;
using DSRS.SharedKernel.Primitives;

namespace DSRS.Application.Interfaces;

public interface IPlayerRepository
{
    Task<bool> NameExistsAsync(string name);
    Task CreateAsync(Player player);
}
