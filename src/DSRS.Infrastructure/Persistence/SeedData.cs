using DSRS.Domain.Items;
using Microsoft.EntityFrameworkCore;

namespace DSRS.Infrastructure.Persistence;

public static class SeedData
{
    public static async Task InitializeAsync(AppDbContext context)
    {
        if (await context.Players.AnyAsync()) return; // DB has been seeded

        await PopulateItemDataAsync(context);
    }

    public static async Task PopulateItemDataAsync(AppDbContext context)
    {
        var list = new List<Item>();

        if (await context.Items.AllAsync(p =>
            p.Name != "Aetherbound Crystal" ||
            p.Name != "Dragon’s Echo Scale" ||
            p.Name != "Chrono Dust" ||
            p.Name != "Moonveil Silk" ||
            p.Name != "Soul-Linked Relic Core" ||
            p.Name != "Eclipse Ember"))
        {
            list.Add(Item.Create("Aetherbound Crystal", "A floating crystal that stores condensed mana. Glows brighter when prices spike", 100, 0.1m).Data!);
            list.Add(Item.Create("Dragon’s Echo Scale", "A scale shed by ancient dragons—still warm, still humming with power. Used for legendary crafting.", 200, 0.1m).Data!);
            list.Add(Item.Create("Chrono Dust", "Sand harvested from collapsed timelines. Can slightly rewind events… or wreck the market.", 300, 0.1m).Data!);
            list.Add(Item.Create("Moonveil Silk", "Fabric woven by nocturnal spirit spiders under a full moon. Light as air, absurdly expensive.", 400, 0.1m).Data!);
            list.Add(Item.Create("Soul-Linked Relic Core", "A mysterious core that bonds to its owner and increases in value the longer it’s held. Risky but profitable.", 500, 0.1m).Data!);
            list.Add(Item.Create("Eclipse Ember", "A black-and-violet flame captured during a solar eclipse. Burns without heat, sells high on rare days.", 600, 0.1m).Data!);

        }
        if (list.Count > 0)
        {
            await context.AddRangeAsync(list);
            await context.SaveChangesAsync();
        }
    }
}
