using DSRS.Application.Features.Players.Create;
using DSRS.Application.Interfaces;
using DSRS.Domain.Players;
using DSRS.SharedKernel.Primitives;
using FluentAssertions;
using Moq;
using System.Data.Common;

namespace DSRS.Application.UnitTests.Features.Players.Create;

public class CreatePlayerHandlerTests
{
    private readonly Mock<IPlayerRepository> _mockPlayerRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly CreatePlayerHandler _handler;

    public CreatePlayerHandlerTests()
    {
        _mockPlayerRepository = new Mock<IPlayerRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new CreatePlayerHandler(_mockPlayerRepository.Object, _mockUnitOfWork.Object);
    }

    private sealed class TestDbException : DbException
    {
        public TestDbException(string message) : base(message) { }
    }

    [Fact]
    public async Task Handle_ReturnsFailure_WhenNameAlreadyExists()
    {
        // Arrange

        _mockPlayerRepository.Setup(r => r.NameExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

        var handler = new CreatePlayerHandler(_mockPlayerRepository.Object, _mockUnitOfWork.Object);
        var command = new CreatePlayerCommand("existing-player", 10m);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Equal("Player.Name.Exists", result.Error!.Code);
        Assert.Contains(command.Name, result.Error.Message);

        _mockPlayerRepository.Verify(r => r.CreateAsync(It.IsAny<Player>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithValidPlayerData_ShouldCreatePlayerSuccessfully()
    {
        // Arrange
        var command = new CreatePlayerCommand("John Doe", 1000m);
        var expectedPlayer = Player.Create("John Doe", 1000m).Data!;
        //expectedPlayer?.Id = Guid.NewGuid();

        _mockPlayerRepository
            .Setup(x => x.CreateAsync(It.IsAny<Player>()));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be("John Doe");
        result.Data!.Balance.Should().Be(1000m);
        _mockPlayerRepository.Verify(x => x.CreateAsync(It.IsAny<Player>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithEmptyPlayerName_ShouldReturnFailureResult()
    {
        // Arrange
        var command = new CreatePlayerCommand("", 1000m);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("Player.Name.Empty");
        result.Error!.Message.Should().Be("Player name cannot be empty");
        _mockPlayerRepository.Verify(x => x.CreateAsync(It.IsAny<Player>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithWhitespacePlayerName_ShouldReturnFailureResult()
    {
        // Arrange
        var command = new CreatePlayerCommand("   ", 500m);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("Player.Name.Empty");
    }

    [Fact]
    public async Task Handle_WithNullPlayerName_ShouldReturnFailureResult()
    {
        // Arrange
        var command = new CreatePlayerCommand(null!, 750m);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(100)]
    [InlineData(10000)]
    public async Task Handle_WithVariousBalances_ShouldCreatePlayerWithCorrectBalance(decimal balance)
    {
        // Arrange
        var command = new CreatePlayerCommand("Test Player", balance);
        var expectedPlayer = Player.Create("Test Player", balance).Data!;
        //expectedPlayer.Id = Guid.NewGuid();

        _mockPlayerRepository
            .Setup(x => x.CreateAsync(It.IsAny<Player>()));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Balance.Should().Be(balance);
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrowsException_ShouldPropagateException()
    {
        // Arrange
        var command = new CreatePlayerCommand("John Doe", 1000m);
        _mockPlayerRepository
            .Setup(x => x.CreateAsync(It.IsAny<Player>()))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoryWithCorrectPlayer()
    {
        // Arrange
        var command = new CreatePlayerCommand("Jane Smith", 2000m);
        Player? capturedPlayer = null;

        _mockPlayerRepository
            .Setup(x => x.CreateAsync(It.IsAny<Player>()))
            .Callback<Player>(p => capturedPlayer = p);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        capturedPlayer.Should().NotBeNull();
        capturedPlayer!.Name.Should().Be("Jane Smith");
        capturedPlayer!.Balance.Should().Be(2000m);
    }

    [Fact]
    public async Task Handle_ReturnsDatabaseError_WhenDbExceptionThrown()
    {
        // Arrange

        _mockPlayerRepository.Setup(r => r.NameExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

        // Make CreateAsync throw a DbException
        _mockPlayerRepository.Setup(r => r.CreateAsync(It.IsAny<Player>()))
                .ThrowsAsync(new TestDbException("simulated db failure"));

        var handler = new CreatePlayerHandler(_mockPlayerRepository.Object, _mockUnitOfWork.Object);
        var command = new CreatePlayerCommand("player", 1m);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Equal("Database.Error", result.Error!.Code);
        Assert.Contains("simulated db failure", result.Error.Message);

        // Commit should not be called because exception occurred before commit
        _mockUnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}