using DSRS.Application.Contracts;
using DSRS.Application.Features.Items;
using DSRS.Application.Features.Market;
using DSRS.Application.Features.Players;
using Microsoft.EntityFrameworkCore;

namespace DSRS.Infrastructure.Persistence.Queries;

public class PlayerQuery(AppDbContext context, ICurrentUserService currentUserService) : IPlayerQuery
{
    private readonly AppDbContext _context = context;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    public async Task<List<PlayerDto>> GetOtherPlayers(string query)
    {
        var players = await _context.Players
            .AsNoTracking()
            .Where(p => !p.Id.Equals(_currentUserService.Id))
            .Where(p => !p.IsGuest)
            .Where(p => string.IsNullOrEmpty(query) || p.Name.Contains(query))
            .Select(p => new PlayerDto
            {
                Id = p.Id,
                Name = p.Name,
            })
            .Take(100)
            .ToListAsync();

        return players;
    }

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
                PurchaseLimit = p.PurchaseLimit,
                IsGuest = p.IsGuest,
                CreatedAt = p.CreatedAt,
                InventoryItems = p.InventoryItems
                    .Select(i => new InventoryDto
                    {
                        Id = i.Id,
                        ItemId = i.ItemId,
                        Quantity = i.Quantity,
                        PurchasePrice = i.PurchasePrice,
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
                PurchaseLimit = p.PurchaseLimit,
                InventoryItems = p.InventoryItems
                    .Select(i => new InventoryDto
                    {
                        Id = i.Id,
                        ItemId = i.ItemId,
                        Quantity = i.Quantity,
                        PurchasePrice = i.PurchasePrice,
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

    public async Task<List<PlayerDto>> GetPlayers(string query)
    {
        var players = await _context.Players
            .AsNoTracking()
            .Where(p => !p.IsGuest)
            .Where(p => string.IsNullOrEmpty(query) || p.Name.Contains(query))
            .Select(p => new PlayerDto
            {
                Id = p.Id,
                Name = p.Name,
            })
            .Take(100)
            .ToListAsync();

        return players;
    }
}
