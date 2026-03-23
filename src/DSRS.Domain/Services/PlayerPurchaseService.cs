using DSRS.Domain.Aggregates.Players;

namespace DSRS.Domain.Services;

public sealed class PlayerPurchaseService
{

    private const int MaxPurchaseLimit = 100;
    private const int DailyIncrease = 25;

    public static void GenerateDailyPurchaseLimit(Player player, DateOnly today)
    {
        if (player.LastLimitGeneration == today)
            return;

        int daysPassed = today.DayNumber - player.LastLimitGeneration.DayNumber;

        if (daysPassed <= 0)
            return;

        int storageToAdd = daysPassed * DailyIncrease;

        player.RegenerateLimit(MaxPurchaseLimit, storageToAdd);

        player.SetLastGeneration(today);
    }
}
