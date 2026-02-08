using DSRS.Domain.Entities;
using DSRS.Infrastructure.Persistence;
using DSRS.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
// NOTE: Adjust the using directives above to match your actual namespaces.

namespace DSRS.Infrastructure.UnitTests;

public class PlayerRepositoryTests
{
    // Example pattern: in-memory DbContext for repository tests
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
    public async Task AddEntity_Should_PersistEntity()
    {
        var options = CreateInMemoryOptions(nameof(AddEntity_Should_PersistEntity));

        // Arrange
        await using (var context = new AppDbContext(options))
        {
            var repo = new PlayerRepository(context); // adjust class name
            var entity = Player.Create("Test", 1000).Data!; // adjust entity
            // Act
            await repo.CreateAsync(entity);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        // Assert persisted
        await using (var context = new AppDbContext(options))
        {
            var saved = await context.Set<Player>().FirstOrDefaultAsync(e => e.Name == "Test", cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(saved);
            Assert.Equal("Test", saved!.Name);
        }
    }

    [Fact]
    public async Task NameExistsAsync_ReturnsTrue_WhenNameExists()
    {
        using var context = CreateContext(nameof(NameExistsAsync_ReturnsTrue_WhenNameExists));
        var player = Player.Create("Alice", 10m).Data!;
        context.Players.Add(player);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var repository = new PlayerRepository(context);
        var exists = await repository.NameExistsAsync("Alice");

        Assert.True(exists);
    }

    [Fact]
    public async Task NameExistsAsync_ReturnsFalse_WhenNameDoesNotExist()
    {
        using var context = CreateContext(nameof(NameExistsAsync_ReturnsFalse_WhenNameDoesNotExist));
        var player = Player.Create("Bob", 5m).Data!;
        context.Players.Add(player);
        await context.SaveChangesAsync();

        var repository = new PlayerRepository(context);
        var exists = await repository.NameExistsAsync("Charlie");

        Assert.False(exists);
    }

    [Fact]
    public async Task NameExistsAsync_IsCaseSensitive()
    {
        using var context = CreateContext(nameof(NameExistsAsync_IsCaseSensitive));
        var player = Player.Create("Alice", 10m).Data!;
        context.Players.Add(player);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var repository = new PlayerRepository(context);
        var exists = await repository.NameExistsAsync("alice");

        Assert.False(exists);
    }

    [Fact]
    public async Task NameExistsAsync_ReturnsFalse_WhenNameIsNull()
    {
        using var context = CreateContext(nameof(NameExistsAsync_ReturnsFalse_WhenNameIsNull));
        var player = Player.Create("Dave", 1m).Data!;
        context.Players.Add(player);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var repository = new PlayerRepository(context);
        var exists = await repository.NameExistsAsync(null!);

        Assert.False(exists);
    }

    //[Fact]
    //public async Task GetById_Should_ReturnEntity_WhenExists()
    //{
    //    var options = CreateInMemoryOptions(nameof(GetById_Should_ReturnEntity_WhenExists));

    //    await using (var context = new AppDbContext(options))
    //    {
    //        context.Set<Player>().Add(Player.Create("Exists", 1000).Data!);
    //        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
    //    }

    //    await using (var context = new AppDbContext(options))
    //    {
    //        var repo = new PlayerRepository(context);
    //        var result = await repo.(1);
    //        Assert.NotNull(result);
    //        Assert.Equal("Exists", result!.Name);
    //    }
    //}

    // Add more tests following the same pattern for Update, Delete, validation, exceptions, etc.
}