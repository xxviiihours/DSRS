using DSRS.Application.Contracts;
using DSRS.Domain.Items;
using DSRS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DSRS.Infrastructure.Repositories;

public class ItemRepository(AppDbContext context) : IItemRepository
{
    private readonly AppDbContext _context = context;

    public async Task<bool> NameExists(string name)
    {
        return await _context.Items.AnyAsync(p => p.Name == name);
    }
    public async Task CreateAsync(Item item)
    {
        await _context.AddAsync(item);
        await Task.CompletedTask;
    }

    public async Task<List<Item>> GetAllAsync()
    {
        return [.. await _context.Items.ToListAsync()];
    }
}
