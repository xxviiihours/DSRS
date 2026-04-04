using DSRS.Application.Contracts;
using DSRS.Application.Features.Market.GetMarketPrices;
using DSRS.Domain.Aggregates.Items;
using DSRS.Domain.Aggregates.Players;
using DSRS.Domain.ValueObjects;
using DSRS.SharedKernel.Enums;
using DSRS.SharedKernel.Interfaces;
using FluentAssertions;
using Moq;

namespace DSRS.Application.UnitTests.Features.Players.Get;

public class GetMarketPriceHandlerTests
{
    private readonly Mock<IPlayerRepository> _mockPlayerRepository;
    private readonly Mock<IItemRepository> _mockItemRepository;
    private readonly Mock<IDateTime> _mockDateTimeService;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly GetMarketPricesHandler _handler;

    public GetMarketPriceHandlerTests()
    {
        _mockPlayerRepository = new Mock<IPlayerRepository>();
        _mockItemRepository = new Mock<IItemRepository>();
        _mockDateTimeService = new Mock<IDateTime>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new GetMarketPricesHandler(
            _mockPlayerRepository.Object,
            _mockItemRepository.Object,
            _mockDateTimeService.Object,
            _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_WithExistingDailyPrices_ShouldReturnPlayerWithoutGeneratingNewPrices()
    {
        // Arrange
        var player = Player.Create("Test Player").Data!;
        var item = Item.Create("Test Item", "A test item", 100m, 0.1m).Data!;
        var today = DateOnly.FromDateTime(DateTime.Now);
        var price = Money.From(100m);

        player.AddDailyPrice(item.Id, today, price, 29m, PriceState.HIGH);

        var command = new GetMarketPricesCommand(player.Id);

        _mockPlayerRepository
            .Setup(r => r.GetByIdWithDailyPrices(player.Id))
            .ReturnsAsync(player);

        _mockDateTimeService
            .Setup(s => s.DateToday)
            .Returns(today);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.DailyPrices.Should().HaveCount(1);
        result.Data!.DailyPrices.First().Price.Should().Be(100m);

        _mockItemRepository.Verify(r => r.GetAllAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_RepositoryReturnsPlayer_ShouldReturnSuccessResult()
    {
        // Arrange
        var player = Player.Create("John Doe").Data!;
        var command = new GetMarketPricesCommand(player.Id);
        var today = DateOnly.FromDateTime(DateTime.Now);

        _mockPlayerRepository
            .Setup(r => r.GetByIdWithDailyPrices(player.Id))
            .ReturnsAsync(player);

        _mockItemRepository
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync([]);

        _mockDateTimeService
            .Setup(s => s.DateToday)
            .Returns(today);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be("John Doe");
        result.Data!.Balance.Should().Be(1000m);
    }

    [Fact]
    public async Task Handle_WithValidPlayerId_ShouldCallPlayerRepository()
    {
        // Arrange
        var player = Player.Create("Player").Data!;
        var command = new GetMarketPricesCommand(player.Id);
        var today = DateOnly.FromDateTime(DateTime.Now);

        _mockPlayerRepository
            .Setup(r => r.GetByIdWithDailyPrices(player.Id))
            .ReturnsAsync(player);

        _mockItemRepository
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync([]);

        _mockDateTimeService
            .Setup(s => s.DateToday)
            .Returns(today);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockPlayerRepository.Verify(
            r => r.GetByIdWithDailyPrices(player.Id),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldPassCorrectPlayerIdToRepository()
    {
        // Arrange
        var player = Player.Create("Player").Data!;
        PlayerId capturedPlayerId = PlayerId.Empty();
        var today = DateOnly.FromDateTime(DateTime.Now);

        _mockPlayerRepository
            .Setup(r => r.GetByIdWithDailyPrices(It.IsAny<PlayerId>()))
            .Callback<PlayerId>(id => capturedPlayerId = id)
            .ReturnsAsync(player);

        _mockItemRepository
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync([]);

        _mockDateTimeService
            .Setup(s => s.DateToday)
            .Returns(today);

        var command = new GetMarketPricesCommand(player.Id);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        capturedPlayerId.Should().Be(player.Id);
    }

    [Fact]
    public async Task Handle_WhenPlayerRepositoryThrowsException_ShouldPropagateException()
    {
        // Arrange
        var command = new GetMarketPricesCommand(PlayerId.New());

        _mockPlayerRepository
            .Setup(r => r.GetByIdWithDailyPrices(It.IsAny<PlayerId>()))
            .ThrowsAsync(new InvalidOperationException("Repository error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithNoDailyPrices_ShouldGeneratePricesForAllItems()
    {
        // Arrange - When a player has no daily prices (Count == 0), prices should be generated
        var playerId = Guid.NewGuid();
        var player = Player.Create("Test Player").Data!;
        var command = new GetMarketPricesCommand(player.Id);
        var today = DateOnly.FromDateTime(DateTime.Now);

        var item1 = Item.Create("Sword", "A sharp blade", 100m, 0.1m).Data!;
        var item2 = Item.Create("Shield", "A protective barrier", 200m, 0.2m).Data!;
        var items = new List<Item> { item1, item2 };

        _mockPlayerRepository
            .Setup(r => r.GetByIdWithDailyPrices(player.Id))
            .ReturnsAsync(player);

        _mockItemRepository
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(items);

        _mockDateTimeService
            .Setup(s => s.DateToday)
            .Returns(today);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        // Item repository should be called to generate prices
        _mockItemRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_CallsItemRepository_WhenPlayerHasNoDailyPrices()
    {
        // Arrange - When Count == 0, item repository should be called
        var playerId = Guid.NewGuid();
        var player = Player.Create("Test Player").Data!;
        var command = new GetMarketPricesCommand(player.Id);
        var today = DateOnly.FromDateTime(DateTime.Now);

        var items = new List<Item>();

        _mockPlayerRepository
            .Setup(r => r.GetByIdWithDailyPrices(player.Id))
            .ReturnsAsync(player);

        _mockItemRepository
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(items);

        _mockDateTimeService
            .Setup(s => s.DateToday)
            .Returns(today);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        // Item repository IS called when player has no daily prices
        _mockItemRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_UsesDateTimeService_WhenGeneratingPrices()
    {
        // Arrange - When player has no daily prices, DateTimeService is used
        var playerId = Guid.NewGuid();
        var player = Player.Create("Test Player").Data!;
        var command = new GetMarketPricesCommand(player.Id);
        var today = DateOnly.FromDateTime(DateTime.Now);

        var item = Item.Create("Test Item", "A test item", 100m, 0.1m).Data!;
        var items = new List<Item> { item };

        _mockPlayerRepository
            .Setup(r => r.GetByIdWithDailyPrices(player.Id))
            .ReturnsAsync(player);

        _mockItemRepository
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(items);

        _mockDateTimeService
            .Setup(s => s.DateToday)
            .Returns(today);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        // DateTime service IS called to get today's date for price generation
        _mockDateTimeService.Verify(s => s.DateToday, Times.AtLeastOnce);
    }

    [Fact]
    public async Task Handle_WithEmptyItemList_CallsRepositoryAndReturnsSuccessfully()
    {
        // Arrange - Even with no items, the repository should be called when Count == 0
        var playerId = Guid.NewGuid();
        var player = Player.Create("Test Player").Data!;
        var command = new GetMarketPricesCommand(player.Id);
        var today = DateOnly.FromDateTime(DateTime.Now);

        _mockPlayerRepository
            .Setup(r => r.GetByIdWithDailyPrices(player.Id))
            .ReturnsAsync(player);

        _mockItemRepository
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync([]);

        _mockDateTimeService
            .Setup(s => s.DateToday)
            .Returns(today);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.DailyPrices.Should().BeEmpty();
        _mockItemRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_PropagatesItemRepositoryException_WhenGeneratingPrices()
    {
        // Arrange - When player has no daily prices and item repository throws,
        // the exception should be propagated
        var playerId = Guid.NewGuid();
        var player = Player.Create("Test Player").Data!;
        var command = new GetMarketPricesCommand(player.Id);

        _mockPlayerRepository
            .Setup(r => r.GetByIdWithDailyPrices(player.Id))
            .ReturnsAsync(player);

        _mockItemRepository
            .Setup(r => r.GetAllAsync())
            .ThrowsAsync(new InvalidOperationException("Item repository error"));

        _mockDateTimeService
            .Setup(s => s.DateToday)
            .Returns(DateOnly.FromDateTime(DateTime.Now));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithLargeBalance_GeneratesPricesSuccessfully()
    {
        // Arrange - Player with large balance but no daily prices should generate prices
        var playerId = Guid.NewGuid();
        var player = Player.Create("Test Player").Data!;
        var command = new GetMarketPricesCommand(player.Id);
        var today = DateOnly.FromDateTime(DateTime.Now);

        var item = Item.Create("Expensive Item", "A costly item", 500m, 0.15m).Data!;

        _mockPlayerRepository
            .Setup(r => r.GetByIdWithDailyPrices(player.Id))
            .ReturnsAsync(player);

        _mockItemRepository
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync([item]);

        _mockDateTimeService
            .Setup(s => s.DateToday)
            .Returns(today);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Balance.Should().Be(1000m);
        _mockItemRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_SkipsGeneratingWhenPlayerAlreadyHasDailyPrices()
    {
        // Arrange - When player already has daily prices (Count > 0), no new prices should be generated
        var playerId = Guid.NewGuid();
        var player = Player.Create("Test Player").Data!;
        var item = Item.Create("Test Item", "A test item", 100m, 0.1m).Data!;
        var today = DateOnly.FromDateTime(DateTime.Now);
        var price = Money.From(100m);

        // Add an existing daily price so Count > 0
        player.AddDailyPrice(item.Id, today, price, 10m, PriceState.HIGH);

        var command = new GetMarketPricesCommand(player.Id);

        _mockPlayerRepository
            .Setup(r => r.GetByIdWithDailyPrices(player.Id))
            .ReturnsAsync(player);

        _mockDateTimeService
            .Setup(s => s.DateToday)
            .Returns(today);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        // Item repository should NOT be called because Count >= 1
        _mockItemRepository.Verify(r => r.GetAllAsync(), Times.Never);
        result.Data!.DailyPrices.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_ReturnedPlayerShouldMaintainOriginalProperties()
    {
        // Arrange - Player properties should be preserved even after generating prices
        var playerId = Guid.NewGuid();
        var playerName = "Test Player";
        var playerBalance = 1000m;
        var player = Player.Create(playerName).Data!;
        var command = new GetMarketPricesCommand(player.Id);
        var today = DateOnly.FromDateTime(DateTime.Now);

        _mockPlayerRepository
            .Setup(r => r.GetByIdWithDailyPrices(player.Id))
            .ReturnsAsync(player);

        _mockItemRepository
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync([]);

        _mockDateTimeService
            .Setup(s => s.DateToday)
            .Returns(today);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be(playerName);
        result.Data!.Balance.Should().Be(playerBalance);
    }
}
