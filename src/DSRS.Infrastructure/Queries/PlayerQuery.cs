using DSRS.Application.Contracts;
using DSRS.Application.Features.Items;
using DSRS.Application.Features.Market;
using DSRS.Application.Features.Players;
using DSRS.Domain.Items;
using DSRS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DSRS.Infrastructure.Queries;

public class PlayerQuery(AppDbContext context) : IPlayerQuery
{
    private readonly AppDbContext _context = context;

    public async Task<PlayerDto> GetPlayerByIdAsync(Guid playerId)
    {
        var player = await _context.Players
            .AsNoTracking()
            .Where(p => p.Id == playerId)
            .Select(p => new PlayerDto
            {
                Id = p.Id,
                Name = p.Name,
                Balance = p.Balance,
                InventoryItems = p.InventoryItems
                    .Select(i => new InventoryDto
                    {
                        Id = i.Id,
                        ItemId = i.ItemId,
                        Quantity = i.Quantity,
                        Item = new ItemDto
                        {
                            Name = i.Item.Name,
                            Description = i.Item.Description,
                            BasePrice = i.Item.BasePrice,
                        }
                    }).ToList()
            })
            .SingleOrDefaultAsync();


        return player!;
    }

    public async Task<PlayerDto> GetPlayerByName(string name)
    {
        var player = await _context.Players
            .AsNoTracking()
            .Where(p => p.Name == name)
            .Select(p => new PlayerDto
            {
                Id = p.Id,
                Name = p.Name,
                Balance = p.Balance,
                InventoryItems = p.InventoryItems
                    .Select(i => new InventoryDto
                    {
                        Id = i.Id,
                        ItemId = i.ItemId,
                        Quantity = i.Quantity,
                        Item = new ItemDto
                        {
                            Name = i.Item.Name,
                            Description = i.Item.Description,
                            BasePrice = i.Item.BasePrice,
                        }
                    }).ToList()
            })
            .SingleOrDefaultAsync();


        return player!;
    }
}
