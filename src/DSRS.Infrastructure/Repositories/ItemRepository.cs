using DSRS.Application.Interfaces;
using DSRS.Domain.Entities;
using DSRS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

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

}
