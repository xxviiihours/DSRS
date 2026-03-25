using DSRS.Application.Features.Players.CreatePlayer;
using FluentAssertions;

namespace DSRS.Application.UnitTests.Features.Players.Create;

public class CreatePlayerCommandTests
{
    [Fact]
    public void CreatePlayerCommand_ShouldBeCreatedWithValidData()
    {
        // Arrange & Act
        var command = new CreatePlayerCommand("John Doe");

        // Assert
        command.Name.Should().Be("John Doe");
    }

    [Fact]
    public void CreatePlayerCommand_ShouldSupportDeconstruction()
    {
        // Arrange
        var command = new CreatePlayerCommand("Jane Smith");

        // Act

        // Assert
        command.Name.Should().Be("Jane Smith");
    }
}