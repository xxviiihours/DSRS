using DSRS.Application.Interfaces;
using DSRS.Domain.Items;
using FluentAssertions;
using Moq;
using System.Data.Common;
using Xunit;
using DSRS.SharedKernel.Interfaces;
using DSRS.Application.Features.Items.Create;

namespace DSRS.Application.UnitTests.Features.Items.Create;

public class CreateItemHandlerTests
{
    #region Helper Methods

    private static CreateItemHandler CreateHandler(
        IItemRepository? itemRepository = null,
        IUnitOfWork? unitOfWork = null,
        IDateTime? dateTime = null)
    {
        itemRepository ??= new Mock<IItemRepository>().Object;
        unitOfWork ??= new Mock<IUnitOfWork>().Object;
        dateTime ??= new Mock<IDateTime>().Object;

        return new CreateItemHandler(itemRepository, unitOfWork, dateTime);
    }

    #endregion

    #region Handle Method - Valid Cases

    [Fact]
    public async Task Handle_WithValidParameters_ReturnsSuccess()
    {
        var repoMock = new Mock<IItemRepository>();
        var uowMock = new Mock<IUnitOfWork>();
        var dateTimeMock = new Mock<IDateTime>();

        repoMock.Setup(r => r.CreateAsync(It.IsAny<Item>())).Returns(Task.CompletedTask);
        uowMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var handler = CreateHandler(repoMock.Object, uowMock.Object, dateTimeMock.Object);
        var command = new CreateItemCommand("Sword", "A sharp combat blade", 50m, 0.3m);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithValidParameters_CreatesItemWithCorrectProperties()
    {
        var repoMock = new Mock<IItemRepository>();
        var uowMock = new Mock<IUnitOfWork>();
        var dateTimeMock = new Mock<IDateTime>();

        Item? capturedItem = null;
        repoMock.Setup(r => r.CreateAsync(It.IsAny<Item>()))
            .Returns((Item item) => { capturedItem = item; return Task.CompletedTask; });
        uowMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var handler = CreateHandler(repoMock.Object, uowMock.Object, dateTimeMock.Object);
        var command = new CreateItemCommand("Shield", "Protective armor", 75m, 0.2m);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be("Shield");
        result.Data.Description.Should().Be("Protective armor");
        result.Data.BasePrice.Should().Be(75m);
        result.Data.Volatility.Should().Be(0.2m);
    }

    [Fact]
    public async Task Handle_WithValidItem_CallsRepositoryCreateAsync()
    {
        var repoMock = new Mock<IItemRepository>();
        var uowMock = new Mock<IUnitOfWork>();
        var dateTimeMock = new Mock<IDateTime>();

        repoMock.Setup(r => r.CreateAsync(It.IsAny<Item>())).Returns(Task.CompletedTask);
        uowMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var handler = CreateHandler(repoMock.Object, uowMock.Object, dateTimeMock.Object);
        var command = new CreateItemCommand("Axe", "A multi-purpose tool", 40m, 0.25m);

        await handler.Handle(command, CancellationToken.None);

        repoMock.Verify(r => r.CreateAsync(It.IsAny<Item>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithValidItem_CallsUnitOfWorkCommitAsync()
    {
        var repoMock = new Mock<IItemRepository>();
        var uowMock = new Mock<IUnitOfWork>();
        var dateTimeMock = new Mock<IDateTime>();

        repoMock.Setup(r => r.CreateAsync(It.IsAny<Item>())).Returns(Task.CompletedTask);
        uowMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var handler = CreateHandler(repoMock.Object, uowMock.Object, dateTimeMock.Object);
        var command = new CreateItemCommand("Bow", "Ranged weapon", 35m, 0.15m);

        await handler.Handle(command, CancellationToken.None);

        uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithValidItem_ReturnsItemData()
    {
        var repoMock = new Mock<IItemRepository>();
        var uowMock = new Mock<IUnitOfWork>();
        var dateTimeMock = new Mock<IDateTime>();

        repoMock.Setup(r => r.CreateAsync(It.IsAny<Item>())).Returns(Task.CompletedTask);
        uowMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var handler = CreateHandler(repoMock.Object, uowMock.Object, dateTimeMock.Object);
        var command = new CreateItemCommand("Dagger", "Small blade", 20m, 0.1m);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Data.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithMinimumValidBasePrice_ReturnsSuccess()
    {
        var repoMock = new Mock<IItemRepository>();
        var uowMock = new Mock<IUnitOfWork>();
        var dateTimeMock = new Mock<IDateTime>();

        repoMock.Setup(r => r.CreateAsync(It.IsAny<Item>())).Returns(Task.CompletedTask);
        uowMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var handler = CreateHandler(repoMock.Object, uowMock.Object, dateTimeMock.Object);
        var command = new CreateItemCommand("Needle", "tiny sewing tool", 0.01m, 0.05m);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.BasePrice.Should().Be(0.01m);
    }

    [Fact]
    public async Task Handle_WithMaximumValidVolatility_ReturnsSuccess()
    {
        var repoMock = new Mock<IItemRepository>();
        var uowMock = new Mock<IUnitOfWork>();
        var dateTimeMock = new Mock<IDateTime>();

        repoMock.Setup(r => r.CreateAsync(It.IsAny<Item>())).Returns(Task.CompletedTask);
        uowMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var handler = CreateHandler(repoMock.Object, uowMock.Object, dateTimeMock.Object);
        var command = new CreateItemCommand("Volatile Item", "Very unstable", 100m, 1m);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Volatility.Should().Be(1m);
    }

    [Fact]
    public async Task Handle_WithZeroVolatility_ReturnsSuccess()
    {
        var repoMock = new Mock<IItemRepository>();
        var uowMock = new Mock<IUnitOfWork>();
        var dateTimeMock = new Mock<IDateTime>();

        repoMock.Setup(r => r.CreateAsync(It.IsAny<Item>())).Returns(Task.CompletedTask);
        uowMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var handler = CreateHandler(repoMock.Object, uowMock.Object, dateTimeMock.Object);
        var command = new CreateItemCommand("Stable Item", "Very stable", 100m, 0m);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Volatility.Should().Be(0m);
    }

    [Fact]
    public async Task Handle_ValidItem_CreatesItemAndCommits_ReturnsSuccess()
    {
        var repoMock = new Mock<IItemRepository>();
        var uowMock = new Mock<IUnitOfWork>();
        var dateTimeMock = new Mock<IDateTime>();

        Item? captured = null;
        repoMock.Setup(r => r.CreateAsync(It.IsAny<Item>()))
            .Returns((Item item) => { captured = item; return Task.CompletedTask; });

        uowMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = CreateHandler(repoMock.Object, uowMock.Object, dateTimeMock.Object);
        var command = new CreateItemCommand("Axe", "A multi-purpose arms capable of chopping woods and head hehe", 10m, 0.25m);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be("Axe");
        result.Data!.Description.Should().Be("A multi-purpose arms capable of chopping woods and head hehe");
        result.Data.BasePrice.Should().Be(10m);
        result.Data.Volatility.Should().Be(0.25m);

        repoMock.Verify(r => r.CreateAsync(It.IsAny<Item>()), Times.Once);
        uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);

        captured.Should().NotBeNull();
        captured!.Name.Should().Be("Axe");
        captured!.Description.Should().Be("A multi-purpose arms capable of chopping woods and head hehe");
    }

    #endregion

    #region Handle Method - Invalid Item Name

    [Fact]
    public async Task Handle_InvalidItemName_ReturnsFailure_AndDoesNotCallRepoOrCommit()
    {
        var repoMock = new Mock<IItemRepository>();
        var uowMock = new Mock<IUnitOfWork>();
        var dateTimeMock = new Mock<IDateTime>();

        var handler = CreateHandler(repoMock.Object, uowMock.Object, dateTimeMock.Object);
        var command = new CreateItemCommand(string.Empty, "A multi-purpose arms capable of chopping woods and head hehe", 1m, 0.1m);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("Item.Name.Empty");
        result.Error.Message.Should().Contain("empty");

        repoMock.Verify(r => r.CreateAsync(It.IsAny<Item>()), Times.Never);
        uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithNullName_ReturnsFailure()
    {
        var repoMock = new Mock<IItemRepository>();
        var uowMock = new Mock<IUnitOfWork>();
        var dateTimeMock = new Mock<IDateTime>();

        var handler = CreateHandler(repoMock.Object, uowMock.Object, dateTimeMock.Object);
        var command = new CreateItemCommand(null!, "Valid description", 1m, 0.1m);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Item.Name.Empty");
    }

    [Fact]
    public async Task Handle_WithWhitespaceName_ReturnsFailure()
    {
        var repoMock = new Mock<IItemRepository>();
        var uowMock = new Mock<IUnitOfWork>();
        var dateTimeMock = new Mock<IDateTime>();

        var handler = CreateHandler(repoMock.Object, uowMock.Object, dateTimeMock.Object);
        var command = new CreateItemCommand("   ", "Valid description", 1m, 0.1m);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Item.Name.Empty");
    }

    #endregion

    #region Handle Method - Invalid Item Description

    [Fact]
    public async Task Handle_InvalidItemDescription_ReturnsFailure_AndDoesNotCallRepoOrCommit()
    {
        var repoMock = new Mock<IItemRepository>();
        var uowMock = new Mock<IUnitOfWork>();
        var dateTimeMock = new Mock<IDateTime>();

        var handler = CreateHandler(repoMock.Object, uowMock.Object, dateTimeMock.Object);
        var command = new CreateItemCommand("Axe", string.Empty, 1m, 0.1m);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("Item.Description.Empty");
        result.Error.Message.Should().Contain("empty");

        repoMock.Verify(r => r.CreateAsync(It.IsAny<Item>()), Times.Never);
        uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithNullDescription_ReturnsFailure()
    {
        var repoMock = new Mock<IItemRepository>();
        var uowMock = new Mock<IUnitOfWork>();
        var dateTimeMock = new Mock<IDateTime>();

        var handler = CreateHandler(repoMock.Object, uowMock.Object, dateTimeMock.Object);
        var command = new CreateItemCommand("Valid Name", null!, 1m, 0.1m);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Item.Description.Empty");
    }

    [Fact]
    public async Task Handle_WithWhitespaceDescription_ReturnsFailure()
    {
        var repoMock = new Mock<IItemRepository>();
        var uowMock = new Mock<IUnitOfWork>();
        var dateTimeMock = new Mock<IDateTime>();

        var handler = CreateHandler(repoMock.Object, uowMock.Object, dateTimeMock.Object);
        var command = new CreateItemCommand("Valid Name", "   ", 1m, 0.1m);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Item.Description.Empty");
    }

    #endregion

    #region Handle Method - Invalid Base Price

    [Fact]
    public async Task Handle_InvalidBasePriceZero_ReturnsFailure_AndDoesNotCallRepoOrCommit()
    {
        var repoMock = new Mock<IItemRepository>();
        var uowMock = new Mock<IUnitOfWork>();
        var dateTimeMock = new Mock<IDateTime>();

        var handler = CreateHandler(repoMock.Object, uowMock.Object, dateTimeMock.Object);
        var command = new CreateItemCommand("Axe", "A valid description", 0m, 0.1m);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("Item.BasePrice.Invalid");
        result.Error.Message.Should().Contain("greater than zero");

        repoMock.Verify(r => r.CreateAsync(It.IsAny<Item>()), Times.Never);
        uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_InvalidBasePriceNegative_ReturnsFailure_AndDoesNotCallRepoOrCommit()
    {
        var repoMock = new Mock<IItemRepository>();
        var uowMock = new Mock<IUnitOfWork>();
        var dateTimeMock = new Mock<IDateTime>();

        var handler = CreateHandler(repoMock.Object, uowMock.Object, dateTimeMock.Object);
        var command = new CreateItemCommand("Axe", "A valid description", -10m, 0.1m);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("Item.BasePrice.Invalid");

        repoMock.Verify(r => r.CreateAsync(It.IsAny<Item>()), Times.Never);
        uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region Handle Method - Invalid Volatility

    [Fact]
    public async Task Handle_InvalidVolatilityNegative_ReturnsFailure_AndDoesNotCallRepoOrCommit()
    {
        var repoMock = new Mock<IItemRepository>();
        var uowMock = new Mock<IUnitOfWork>();
        var dateTimeMock = new Mock<IDateTime>();

        var handler = CreateHandler(repoMock.Object, uowMock.Object, dateTimeMock.Object);
        var command = new CreateItemCommand("Axe", "A valid description", 10m, -0.1m);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("Item.Volatility.Invalid");
        result.Error.Message.Should().Contain("volatility");

        repoMock.Verify(r => r.CreateAsync(It.IsAny<Item>()), Times.Never);
        uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_InvalidVolatilityGreaterThanOne_ReturnsFailure_AndDoesNotCallRepoOrCommit()
    {
        var repoMock = new Mock<IItemRepository>();
        var uowMock = new Mock<IUnitOfWork>();
        var dateTimeMock = new Mock<IDateTime>();

        var handler = CreateHandler(repoMock.Object, uowMock.Object, dateTimeMock.Object);
        var command = new CreateItemCommand("Axe", "A valid description", 10m, 1.5m);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("Item.Volatility.Invalid");

        repoMock.Verify(r => r.CreateAsync(It.IsAny<Item>()), Times.Never);
        uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region Handle Method - Exception Handling

    private class TestDbException(string message) : DbException(message)
    {
    }

    [Fact]
    public async Task Handle_RepositoryThrowsDbException_ReturnsDatabaseError()
    {
        var repoMock = new Mock<IItemRepository>();
        var uowMock = new Mock<IUnitOfWork>();
        var dateTimeMock = new Mock<IDateTime>();

        repoMock.Setup(r => r.CreateAsync(It.IsAny<Item>()))
            .Throws(new TestDbException("boom"));

        var handler = CreateHandler(repoMock.Object, uowMock.Object, dateTimeMock.Object);
        var command = new CreateItemCommand("Shield", "A sturdy secondary weapon capable of blocking heavy attacks", 15m, 0.3m);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("Database.Error");
        result.Error.Message.Should().Contain("boom");

        repoMock.Verify(r => r.CreateAsync(It.IsAny<Item>()), Times.Once);
        uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_CommitAsyncThrowsDbException_ReturnsDatabaseError()
    {
        var repoMock = new Mock<IItemRepository>();
        var uowMock = new Mock<IUnitOfWork>();
        var dateTimeMock = new Mock<IDateTime>();

        repoMock.Setup(r => r.CreateAsync(It.IsAny<Item>()))
            .Returns(Task.CompletedTask);

        uowMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
            .Throws(new TestDbException("commit failed"));

        var handler = CreateHandler(repoMock.Object, uowMock.Object, dateTimeMock.Object);
        var command = new CreateItemCommand("Sword", "A sharp blade for combat", 20m, 0.5m);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("Database.Error");
        result.Error.Message.Should().Contain("commit failed");

        repoMock.Verify(r => r.CreateAsync(It.IsAny<Item>()), Times.Once);
        uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region Handle Method - Async and Cancellation

    [Fact]
    public async Task Handle_CancellationTokenPropagated_ToCommitAsync()
    {
        var repoMock = new Mock<IItemRepository>();
        var uowMock = new Mock<IUnitOfWork>();
        var dateTimeMock = new Mock<IDateTime>();

        repoMock.Setup(r => r.CreateAsync(It.IsAny<Item>()))
            .Returns(Task.CompletedTask);

        uowMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = CreateHandler(repoMock.Object, uowMock.Object, dateTimeMock.Object);
        var command = new CreateItemCommand("Lance", "A piercing weapon", 18m, 0.25m);
        var cancellationToken = new CancellationToken();

        var result = await handler.Handle(command, cancellationToken);

        result.IsSuccess.Should().BeTrue();
        uowMock.Verify(u => u.CommitAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_IsAsync_CompletesSuccessfully()
    {
        var repoMock = new Mock<IItemRepository>();
        var uowMock = new Mock<IUnitOfWork>();
        var dateTimeMock = new Mock<IDateTime>();

        repoMock.Setup(r => r.CreateAsync(It.IsAny<Item>()))
            .Returns(Task.CompletedTask);
        uowMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = CreateHandler(repoMock.Object, uowMock.Object, dateTimeMock.Object);
        var command = new CreateItemCommand("Spear", "A long pole weapon", 25m, 0.2m);

        // Should not throw
        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    #endregion

    #region Handle Method - Return Values

    [Fact]
    public async Task Handle_Success_ResultDataNotNull()
    {
        var repoMock = new Mock<IItemRepository>();
        var uowMock = new Mock<IUnitOfWork>();
        var dateTimeMock = new Mock<IDateTime>();

        repoMock.Setup(r => r.CreateAsync(It.IsAny<Item>())).Returns(Task.CompletedTask);
        uowMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var handler = CreateHandler(repoMock.Object, uowMock.Object, dateTimeMock.Object);
        var command = new CreateItemCommand("Helmet", "Head protection", 30m, 0.15m);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Error.Should().BeNull();
    }

    [Fact]
    public async Task Handle_Failure_ResultErrorNotNull()
    {
        var repoMock = new Mock<IItemRepository>();
        var uowMock = new Mock<IUnitOfWork>();
        var dateTimeMock = new Mock<IDateTime>();

        var handler = CreateHandler(repoMock.Object, uowMock.Object, dateTimeMock.Object);
        var command = new CreateItemCommand("", "Invalid name", 10m, 0.1m);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
    }

    #endregion
}
