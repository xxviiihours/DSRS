using DSRS.Application.Contracts;
using DSRS.Application.Features.Players;
using DSRS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DSRS.Infrastructure.Queries;

public class PlayerQuery(AppDbContext context) : IPlayerQuery
{
    private readonly AppDbContext _context = context;

    public Task<PlayerDto> GetPlayerByIdAsync(Guid playerId)
    {
        throw new NotImplementedException();
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
            })
            .SingleOrDefaultAsync();


        return player!;
    }
}
