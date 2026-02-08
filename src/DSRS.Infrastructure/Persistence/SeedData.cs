using DSRS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DSRS.Infrastructure.Persistence;

public static class SeedData
{
    public static async Task InitializeAsync(AppDbContext context)
    {
        if (await context.Players.AnyAsync()) return; // DB has been seeded

        await PopulateTestDataAsync(context);
    }

    public static async Task PopulateTestDataAsync(AppDbContext context)
    {
        var list = new List<Player>();

        if (await context.Players.AllAsync(p => p.Name != "Kevin"))
        {
            Player player = Player.Create("Kevin", 1000).Data!;
            list.Add(player);
        }
        if (list.Count > 0)
        {
            await context.AddRangeAsync(list);
            await context.SaveChangesAsync();
        }
    }
}
