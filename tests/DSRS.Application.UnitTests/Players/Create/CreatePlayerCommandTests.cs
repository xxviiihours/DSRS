using DSRS.Application.Players.Create;
using FluentAssertions;

namespace DSRS.Application.UnitTests.Players.Create;

public class CreatePlayerCommandTests
{
    [Fact]
    public void CreatePlayerCommand_ShouldBeCreatedWithValidData()
    {
        // Arrange & Act
        var command = new CreatePlayerCommand("John Doe", 1000m);

        // Assert
        command.Name.Should().Be("John Doe");
        command.Balance.Should().Be(1000m);
    }

    [Fact]
    public void CreatePlayerCommand_ShouldSupportDeconstruction()
    {
        // Arrange
        var command = new CreatePlayerCommand("Jane Smith", 2000m);

        // Act
        var (name, balance) = command;

        // Assert
        name.Should().Be("Jane Smith");
        balance.Should().Be(2000m);
    }
}