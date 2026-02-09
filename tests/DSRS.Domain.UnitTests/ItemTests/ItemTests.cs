using DSRS.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace DSRS.Domain.UnitTests.ItemTests;

public class ItemTests
{
    [Fact]
    public void Create_WithValidParameters_ReturnsSuccess()
    {
        // Arrange
        var name = "Sword";
        var basePrice = 10m;
        var volatility = 0.2m;

        // Act
        var result = Item.Create(name, basePrice, volatility);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be(name);
        result.Data.BasePrice.Should().Be(basePrice);
        result.Data.Volatility.Should().Be(volatility);
    }

    [Fact]
    public void Create_WithEmptyName_ReturnsFailure()
    {
        var result = Item.Create(string.Empty, 1m, 0.1m);
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("Item.Name.Empty");
    }

    [Fact]
    public void Create_WithNonPositiveBasePrice_ReturnsFailure()
    {
        var result = Item.Create("Axe", 0m, 0.1m);
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("Item.BasePrice.Invalid");
    }

    [Fact]
    public void Create_WithVolatilityLessThanZero_ReturnsFailure()
    {
        var result = Item.Create("Bow", 5m, -0.01m);
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("Item.Volatility.Invalid");
    }

    [Fact]
    public void Create_WithVolatilityGreaterThanOne_ReturnsFailure()
    {
        var result = Item.Create("Arrow", 1m, 1.01m);
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("Item.Volatility.Invalid");
    }

    [Fact]
    public void Create_WithVolatilityZero_IsValid()
    {
        var result = Item.Create("Gem", 2m, 0m);
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Volatility.Should().Be(0m);
    }

    [Fact]
    public void Create_WithVolatilityOne_IsValid()
    {
        var result = Item.Create("Gem", 2m, 1m);
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Volatility.Should().Be(1m);
    }
}
