using System;

namespace DSRS.Domain.Aggregates.Players;

public sealed class PlayerPurchaseService
{
  private const int DailyIncrease = 25;

  public static void GenerateDailyPurchaseLimit(Player player, DateOnly today)
  {
    if (player.LastLimitGeneration == today)
      return;

    int daysPassed = today.DayNumber - player.LastLimitGeneration.DayNumber;

    if (daysPassed <= 0)
      return;

    int storageToAdd = daysPassed * DailyIncrease;

    player.RegenerateLimit(storageToAdd);

    player.SetLastGeneration(today);
  }
}
