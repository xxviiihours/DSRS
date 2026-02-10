using DSRS.Domain.Entities;
using DSRS.Infrastructure.Persistence;
using DSRS.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DSRS.Infrastructure.UnitTests;

public class ItemRepositoryTests
{
    private static DbContextOptions<AppDbContext> CreateInMemoryOptions(string dbName)
        => new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

    private static AppDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task NameExists_ReturnsTrue_WhenNameExists()
    {
        // Arrange
        using var context = CreateContext(nameof(NameExists_ReturnsTrue_WhenNameExists));
        var item = Item.Create("Iron Ore", 100m, 0.5m).Data!;
        context.Items.Add(item);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var repository = new ItemRepository(context);

        // Act
        var exists = await repository.NameExists("Iron Ore");

        // Assert
        Assert.True(exists);
    }

    [Fact]
    public async Task NameExists_ReturnsFalse_WhenNameDoesNotExist()
    {
        // Arrange
        using var context = CreateContext(nameof(NameExists_ReturnsFalse_WhenNameDoesNotExist));
        var item = Item.Create("Gold Bar", 500m, 0.3m).Data!;
        context.Items.Add(item);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var repository = new ItemRepository(context);

        // Act
        var exists = await repository.NameExists("Silver Coin");

        // Assert
        Assert.False(exists);
    }

    [Fact]
    public async Task NameExists_IsCaseSensitive()
    {
        // Arrange
        using var context = CreateContext(nameof(NameExists_IsCaseSensitive));
        var item = Item.Create("Diamond", 1000m, 0.7m).Data!;
        context.Items.Add(item);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var repository = new ItemRepository(context);

        // Act
        var exists = await repository.NameExists("diamond");

        // Assert
        Assert.False(exists);
    }

    [Fact]
    public async Task NameExists_ReturnsFalse_WhenNameIsNull()
    {
        // Arrange
        using var context = CreateContext(nameof(NameExists_ReturnsFalse_WhenNameIsNull));
        var item = Item.Create("Copper Ore", 50m, 0.4m).Data!;
        context.Items.Add(item);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var repository = new ItemRepository(context);

        // Act
        var exists = await repository.NameExists(null!);

        // Assert
        Assert.False(exists);
    }

    [Fact]
    public async Task NameExists_ReturnsFalse_WhenDatabaseIsEmpty()
    {
        // Arrange
        using var context = CreateContext(nameof(NameExists_ReturnsFalse_WhenDatabaseIsEmpty));
        var repository = new ItemRepository(context);

        // Act
        var exists = await repository.NameExists("Any Item");

        // Assert
        Assert.False(exists);
    }

    [Fact]
    public async Task CreateAsync_Should_PersistEntity()
    {
        // Arrange
        var options = CreateInMemoryOptions(nameof(CreateAsync_Should_PersistEntity));
        Item? savedItem;

        await using (var context = new AppDbContext(options))
        {
            var repository = new ItemRepository(context);
            var item = Item.Create("Emerald", 750m, 0.6m).Data!;

            // Act
            await repository.CreateAsync(item);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        // Assert
        await using (var context = new AppDbContext(options))
        {
            savedItem = await context.Set<Item>()
                .FirstOrDefaultAsync(i => i.Name == "Emerald", 
                    cancellationToken: TestContext.Current.CancellationToken);
        }

        Assert.NotNull(savedItem);
        Assert.Equal("Emerald", savedItem!.Name);
    }

    [Fact]
    public async Task CreateAsync_Should_PreserveAllItemProperties()
    {
        // Arrange
        const string itemName = "Sapphire";
        const decimal basePrice = 600m;
        const decimal volatility = 0.55m;

        var options = CreateInMemoryOptions(nameof(CreateAsync_Should_PreserveAllItemProperties));
        Item? savedItem;

        await using (var context = new AppDbContext(options))
        {
            var repository = new ItemRepository(context);
            var item = Item.Create(itemName, basePrice, volatility).Data!;

            // Act
            await repository.CreateAsync(item);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        // Assert
        await using (var context = new AppDbContext(options))
        {
            savedItem = await context.Set<Item>()
                .FirstOrDefaultAsync(i => i.Name == itemName, 
                    cancellationToken: TestContext.Current.CancellationToken);
        }

        Assert.NotNull(savedItem);
        Assert.Equal(itemName, savedItem!.Name);
        Assert.Equal(basePrice, savedItem.BasePrice);
        Assert.Equal(volatility, savedItem.Volatility);
    }

    [Fact]
    public async Task CreateAsync_Should_AssignIdToEntity()
    {
        // Arrange
        var options = CreateInMemoryOptions(nameof(CreateAsync_Should_AssignIdToEntity));
        Item? savedItem;

        await using (var context = new AppDbContext(options))
        {
            var repository = new ItemRepository(context);
            var item = Item.Create("Ruby", 800m, 0.65m).Data!;

            // Act
            await repository.CreateAsync(item);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        // Assert
        await using (var context = new AppDbContext(options))
        {
            savedItem = await context.Set<Item>()
                .FirstOrDefaultAsync(i => i.Name == "Ruby", 
                    cancellationToken: TestContext.Current.CancellationToken);
        }

        Assert.NotNull(savedItem);
        Assert.NotEqual(Guid.Empty, savedItem!.Id);
    }

    [Fact]
    public async Task CreateAsync_Should_AllowMultipleItems()
    {
        // Arrange
        var options = CreateInMemoryOptions(nameof(CreateAsync_Should_AllowMultipleItems));
        int savedItemCount;

        await using (var context = new AppDbContext(options))
        {
            var repository = new ItemRepository(context);
            var item1 = Item.Create("Coal", 25m, 0.3m).Data!;
            var item2 = Item.Create("Tin", 75m, 0.4m).Data!;
            var item3 = Item.Create("Lead", 40m, 0.35m).Data!;

            // Act
            await repository.CreateAsync(item1);
            await repository.CreateAsync(item2);
            await repository.CreateAsync(item3);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        // Assert
        await using (var context = new AppDbContext(options))
        {
            savedItemCount = await context.Set<Item>().CountAsync(cancellationToken: TestContext.Current.CancellationToken);
        }

        Assert.Equal(3, savedItemCount);
    }
}
