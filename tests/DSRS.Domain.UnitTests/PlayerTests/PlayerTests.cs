using DSRS.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace DSRS.Domain.UnitTests.PlayerTests;

public class PlayerTests
{
    [Fact]
    public void Create_WithValidParameters_ReturnsSuccess()
    {
        var name = "Alice";
        var balance = 100m;

        var result = Player.Create(name, balance);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be(name);
        result.Data.Balance.Should().Be(balance);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void Create_WithEmptyOrWhitespaceName_ReturnsFailure(string name)
    {
        var result = Player.Create(name, 0m);
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("Player.Name.Empty");
    }

    [Fact]
    public void Create_WithNegativeBalance_AllowsNegativeBalance()
    {
        var result = Player.Create("Bob", -50m);
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Balance.Should().Be(-50m);
    }
}
