using System;

namespace DSRS.Domain.Players;

public sealed class PlayerStorageService
{
  private const int DailyIncrease = 25;

  public static void GenerateDailyStorage(Player player, DateOnly today)
  {
    if (player.LastStorageGeneration == today)
      return;

    int daysPassed = today.DayNumber - player.LastStorageGeneration.DayNumber;

    if (daysPassed <= 0)
      return;

    int storageToAdd = daysPassed * DailyIncrease;

    player.IncreaseStorage(storageToAdd);

    player.SetLastGeneration(today);
  }
}
