using DSRS.Domain.Aggregates.Items;
using DSRS.Domain.Aggregates.Players;
using DSRS.Domain.Aggregates.Pricing;
using DSRS.Domain.ValueObjects;
using DSRS.Infrastructure.Persistence;
using DSRS.Infrastructure.Persistence.Repositories;
using DSRS.SharedKernel.Enums;
using DSRS.SharedKernel.Interfaces;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace DSRS.Infrastructure.UnitTests;

public class DailyPriceRepositoryTests
{
    private static DbContextOptions<AppDbContext> CreateInMemoryOptions(string dbName)
        => new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
    private static readonly Mock<IDateTime> _mockDateTime = new();
    private static AppDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new AppDbContext(options, _mockDateTime.Object);
    }

    [Fact]
    public async Task CreateAsync_Should_AddDailyPriceToContext()
    {
        // Arrange
        using var context = CreateContext(nameof(CreateAsync_Should_AddDailyPriceToContext));
        var player = Player.Create("Test Player").Data!;
        var item = Item.Create("Iron Ore", "Mining resource", 100m, 0.5m).Data!;
        context.Items.Add(item);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var dailyPrice = DailyPrice.Create(player.Id, item.Id, DateOnly.FromDateTime(DateTime.Now), Money.From(105m), 12m, PriceState.HIGH).Data!;
        var repository = new DailyPriceRepository(context);

        // Act
        await repository.CreateAsync(dailyPrice);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        // Assert
        var savedPrice = await context.Set<DailyPrice>()
            .FirstOrDefaultAsync(dp => dp.ItemId == item.Id, cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(savedPrice);
        Assert.Equal(item.Id, savedPrice!.ItemId);
        Assert.Equal(105m, savedPrice.Price.Value);
        Assert.Equal(PriceState.HIGH, savedPrice.State);
    }

    [Fact]
    public async Task CreateAsync_Should_PersistDailyPrice_WhenSaveChangesIsCalled()
    {
        var options = CreateInMemoryOptions(nameof(CreateAsync_Should_PersistDailyPrice_WhenSaveChangesIsCalled));

        // Arrange & Act
        await using (var context = new AppDbContext(options, _mockDateTime.Object))
        {
            var player = Player.Create("Test Player").Data!;
            var item = Item.Create("Gold Bar", "Precious metal", 500m, 0.3m).Data!;
            context.Items.Add(item);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var dailyPrice = DailyPrice.Create(player.Id, item.Id, DateOnly.FromDateTime(DateTime.Now), Money.From(520m), 20m, PriceState.HIGH).Data!;
            var repository = new DailyPriceRepository(context);
            await repository.CreateAsync(dailyPrice);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        // Assert persisted
        await using (var context = new AppDbContext(options, _mockDateTime.Object))
        {
            var savedPrice = await context.Set<DailyPrice>()
                .FirstOrDefaultAsync(dp => dp.Price.Value == 520m, cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(savedPrice);
            Assert.Equal(520m, savedPrice!.Price.Value);
        }
    }

    [Fact]
    public async Task CreateAsync_Should_MaintainDailyPriceProperties()
    {
        // Arrange
        using var context = CreateContext(nameof(CreateAsync_Should_MaintainDailyPriceProperties));
        var player = Player.Create("Test Player").Data!;
        var item = Item.Create("Copper Ore", "Metal ore", 75m, 0.4m).Data!;
        context.Items.Add(item);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var date = new DateOnly(2026, 2, 13);
        var dailyPrice = DailyPrice.Create(player.Id, item.Id, date, Money.From(78m), 20m, PriceState.LOW).Data!;
        var repository = new DailyPriceRepository(context);

        // Act
        await repository.CreateAsync(dailyPrice);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        // Assert
        var savedPrice = await context.Set<DailyPrice>()
            .FirstOrDefaultAsync(dp => dp.ItemId == item.Id, cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(savedPrice);
        Assert.Equal(date, savedPrice!.Date);
        Assert.Equal(78m, savedPrice.Price.Value);
        Assert.Equal(PriceState.LOW, savedPrice.State);
    }

    [Fact]
    public async Task CreateAllAsync_Should_AddMultipleDailyPricesToContext()
    {
        // Arrange
        using var context = CreateContext(nameof(CreateAllAsync_Should_AddMultipleDailyPricesToContext));
        var player = Player.Create("Test Player").Data!;
        var item1 = Item.Create("Silver Ore", "Precious metal ore", 150m, 0.6m).Data!;
        var item2 = Item.Create("Tin Ore", "Metal ore", 50m, 0.2m).Data!;
        context.Items.AddRange(item1, item2);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var date = DateOnly.FromDateTime(DateTime.Now);
        var dailyPrices = new List<DailyPrice>
        {
            DailyPrice.Create(player.Id, item1.Id, date, Money.From(155m), 10m, PriceState.HIGH).Data!,
            DailyPrice.Create(player.Id, item2.Id, date, Money.From(48m),25m, PriceState.LOW).Data!
        };
        var repository = new DailyPriceRepository(context);

        // Act
        await repository.CreateAllAsync(dailyPrices);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Assert
        var savedPrices = await context.Set<DailyPrice>()
            .Where(dp => dp.ItemId == item1.Id || dp.ItemId == item2.Id)
            .ToListAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(2, savedPrices.Count);
        Assert.Contains(savedPrices, dp => dp.ItemId == item1.Id && dp.Price.Value == 155m);
        Assert.Contains(savedPrices, dp => dp.ItemId == item2.Id && dp.Price.Value == 48m);
    }

    [Fact]
    public async Task CreateAllAsync_Should_PersistMultipleDailyPrices_WhenSaveChangesIsCalled()
    {
        var options = CreateInMemoryOptions(nameof(CreateAllAsync_Should_PersistMultipleDailyPrices_WhenSaveChangesIsCalled));

        // Arrange & Act
        await using (var context = new AppDbContext(options, _mockDateTime.Object))
        {
            var player = Player.Create("Test Player").Data!;
            var item1 = Item.Create("Diamond", "Precious stone", 1000m, 0.8m).Data!;
            var item2 = Item.Create("Emerald", "Precious stone", 800m, 0.7m).Data!;
            context.Items.AddRange(item1, item2);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var date = DateOnly.FromDateTime(DateTime.Now);
            var dailyPrices = new List<DailyPrice>
            {
                DailyPrice.Create(player.Id, item1.Id, date, Money.From(1050m),20m, PriceState.HIGH).Data!,
                DailyPrice.Create(player.Id, item2.Id, date, Money.From(820m),30m, PriceState.HIGH).Data!
            };
            var repository = new DailyPriceRepository(context);
            await repository.CreateAllAsync(dailyPrices);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        // Assert persisted
        await using (var context = new AppDbContext(options, _mockDateTime.Object))
        {
            var savedPrices = await context.Set<DailyPrice>()
                .Where(dp => dp.Price == 1050m || dp.Price == 820m)
                .ToListAsync(cancellationToken: TestContext.Current.CancellationToken);
            Assert.Equal(2, savedPrices.Count);
            Assert.All(savedPrices, dp => Assert.Equal(PriceState.HIGH, dp.State));
        }
    }

    [Fact]
    public async Task CreateAllAsync_Should_HandleEmptyList()
    {
        // Arrange
        using var context = CreateContext(nameof(CreateAllAsync_Should_HandleEmptyList));
        var dailyPrices = new List<DailyPrice>();
        var repository = new DailyPriceRepository(context);
        var initialCount = await context.Set<DailyPrice>().CountAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Act
        await repository.CreateAllAsync(dailyPrices);

        // Assert
        var finalCount = await context.Set<DailyPrice>().CountAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(initialCount, finalCount);
    }

    [Fact]
    public async Task CreateAllAsync_Should_MaintainAllDailyPriceProperties()
    {
        // Arrange
        using var context = CreateContext(nameof(CreateAllAsync_Should_MaintainAllDailyPriceProperties));
        var player = Player.Create("Test Player").Data!;
        var item1 = Item.Create("Platinum", "Precious metal", 1200m, 0.5m).Data!;
        var item2 = Item.Create("Palladium", "Precious metal", 900m, 0.4m).Data!;
        context.Items.AddRange(item1, item2);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var date = new DateOnly(2026, 2, 13);
        var dailyPrices = new List<DailyPrice>
        {
            DailyPrice.Create(player.Id, item1.Id, date.AddDays(-1), Money.From(1210m),10m, PriceState.HIGH).Data!,
            DailyPrice.Create(player.Id, item2.Id, date, Money.From(890m),-10m, PriceState.LOW).Data!
        };
        var repository = new DailyPriceRepository(context);

        // Act
        await repository.CreateAllAsync(dailyPrices);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Assert
        var savedPrices = await context.Set<DailyPrice>()
            .Where(dp => dp.ItemId == item1.Id || dp.ItemId == item2.Id)
            .ToListAsync(cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(2, savedPrices.Count);

        var price1 = savedPrices.First(dp => dp.ItemId == item1.Id);
        Assert.Equal(date.AddDays(-1), price1.Date);
        Assert.Equal(1210m, price1.Price.Value);
        Assert.Equal(PriceState.HIGH, price1.State);

        var price2 = savedPrices.First(dp => dp.ItemId == item2.Id);
        Assert.Equal(date, price2.Date);
        Assert.Equal(890m, price2.Price.Value);
        Assert.Equal(PriceState.LOW, price2.State);
    }
}
