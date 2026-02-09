using DSRS.Application.Items.Create;
using DSRS.Application.Interfaces;
using DSRS.Domain.Entities;
using DSRS.SharedKernel.Primitives;
using FluentAssertions;
using Moq;
using System.Data.Common;
using Xunit;

namespace DSRS.Application.UnitTests.Items.Create;

public class CreateItemHandlerTests
{

    [Fact]
    public async Task Handle_InvalidItem_ReturnsFailure_AndDoesNotCallRepoOrCommit()
    {
        var repoMock = new Mock<IItemRepository>();
        var uowMock = new Mock<IUnitOfWork>();

        var handler = new CreateItemHandler(repoMock.Object, uowMock.Object);

        var command = new CreateItemCommand(string.Empty, 1m, 0.1m); // invalid name

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("Item.Name.Empty");

        repoMock.Verify(r => r.Create(It.IsAny<Item>()), Times.Never);
        uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidItem_CreatesItemAndCommits_ReturnsSuccess()
    {
        var repoMock = new Mock<IItemRepository>();
        var uowMock = new Mock<IUnitOfWork>();

        Item? captured = null;
        repoMock.Setup(r => r.Create(It.IsAny<Item>()))
            .Returns((Item item) => { captured = item; return Task.CompletedTask; });

        uowMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new CreateItemHandler(repoMock.Object, uowMock.Object);
        var command = new CreateItemCommand("Axe", 10m, 0.25m);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be("Axe");
        result.Data.BasePrice.Should().Be(10m);
        result.Data.Volatility.Should().Be(0.25m);

        repoMock.Verify(r => r.Create(It.IsAny<Item>()), Times.Once);
        uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);

        captured.Should().NotBeNull();
        captured!.Name.Should().Be("Axe");
    }

    private class TestDbException : DbException
    {
        public TestDbException(string message) : base(message) { }
    }

    [Fact]
    public async Task Handle_RepositoryThrowsDbException_ReturnsDatabaseError()
    {
        var repoMock = new Mock<IItemRepository>();
        var uowMock = new Mock<IUnitOfWork>();

        repoMock.Setup(r => r.Create(It.IsAny<Item>()))
            .Throws(new TestDbException("boom"));

        var handler = new CreateItemHandler(repoMock.Object, uowMock.Object);
        var command = new CreateItemCommand("Shield", 15m, 0.3m);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("Database.Error");
        result.Error.Message.Should().Contain("boom");

        repoMock.Verify(r => r.Create(It.IsAny<Item>()), Times.Once);
        uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
