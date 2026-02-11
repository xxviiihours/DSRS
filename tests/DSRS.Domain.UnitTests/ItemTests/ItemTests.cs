using DSRS.Domain.Items;
using DSRS.SharedKernel.Primitives;
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
        var description = "sturdy weapon capable of mass destruction";
        var basePrice = 10m;
        var volatility = 0.2m;

        // Act
        var result = Item.Create(name, description, basePrice, volatility);

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
        var result = Item.Create(string.Empty, "a sturdy weapon capable of mass destruction", 1m, 0.1m);
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("Item.Name.Empty");
    }

    [Fact]
    public void Create_WithEmptDescription_ReturnsFailure()
    {
        var result = Item.Create("Sword", string.Empty, 1m, 0.1m);
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("Item.Description.Empty");
    }
    [Fact]
    public void Create_WithNonPositiveBasePrice_ReturnsFailure()
    {
        var result = Item.Create("Axe", "A multi-purpose arms capable of chopping woods and head hehe", 0m, 0.1m);
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("Item.BasePrice.Invalid");
    }

    [Fact]
    public void Create_WithVolatilityLessThanZero_ReturnsFailure()
    {
        var result = Item.Create("Bow", "Long range weapon good for hunting", 5m, -0.01m);
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("Item.Volatility.Invalid");
    }

    [Fact]
    public void Create_WithVolatilityGreaterThanOne_ReturnsFailure()
    {
        var result = Item.Create("Arrow", "A sturdy ammunition capable of piercing heavy armors", 1m, 1.01m);
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("Item.Volatility.Invalid");
    }

    [Fact]
    public void Create_WithVolatilityZero_IsValid()
    {
        var result = Item.Create("Gem", "A jewel that can fetch high price", 2m, 0m);
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Volatility.Should().Be(0m);
    }

    [Fact]
    public void Create_WithVolatilityOne_IsValid()
    {
        var result = Item.Create("Gem", "A jewel that can fetch a high price", 2m, 1m);
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Volatility.Should().Be(1m);
    }

    [Fact]
    public void Create_WithWhitespaceOnlyName_ReturnsFailure()
    {
        var result = Item.Create("   ", "A valid description", 10m, 0.2m);
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("Item.Name.Empty");
    }

    [Fact]
    public void Create_WithWhitespaceOnlyDescription_ReturnsFailure()
    {
        var result = Item.Create("Valid Name", "   ", 10m, 0.2m);
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("Item.Description.Empty");
    }

    [Fact]
    public void Create_WithNullName_ReturnsFailure()
    {
        var result = Item.Create(null!, "A valid description", 10m, 0.2m);
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("Item.Name.Empty");
    }

    [Fact]
    public void Create_WithNullDescription_ReturnsFailure()
    {
        var result = Item.Create("Valid Name", null!, 10m, 0.2m);
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("Item.Description.Empty");
    }

    [Fact]
    public void Create_WithNegativeBasePrice_ReturnsFailure()
    {
        var result = Item.Create("Axe", "A tool", -50m, 0.1m);
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("Item.BasePrice.Invalid");
    }

    [Fact]
    public void Create_WithVerySmallBasePrice_ReturnsSuccess()
    {
        var result = Item.Create("Needle", "A tiny sewing tool", 0.01m, 0.05m);
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.BasePrice.Should().Be(0.01m);
    }

    [Fact]
    public void Create_WithVeryLargeBasePrice_ReturnsSuccess()
    {
        var result = Item.Create("Crown Jewel", "Priceless artifact", 999999999m, 0.5m);
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.BasePrice.Should().Be(999999999m);
    }

    [Fact]
    public void Create_WithAllValidParametersMaxValues_ReturnsSuccess()
    {
        var result = Item.Create("Ultimate Item", "The greatest item ever", 999999m, 1m);
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be("Ultimate Item");
        result.Data!.Description.Should().Be("The greatest item ever");
        result.Data!.BasePrice.Should().Be(999999m);
        result.Data!.Volatility.Should().Be(1m);
    }

    [Fact]
    public void Create_WithAllValidParametersMinValues_ReturnsSuccess()
    {
        var result = Item.Create("A", "B", 0.001m, 0m);
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be("A");
        result.Data!.Description.Should().Be("B");
        result.Data!.BasePrice.Should().Be(0.001m);
        result.Data!.Volatility.Should().Be(0m);
    }

    [Fact]
    public void Create_ValidItem_AllPropertiesSetCorrectly()
    {
        // Arrange
        var name = "Legendary Sword";
        var description = "A sword forged in ancient times";
        var basePrice = 5000m;
        var volatility = 0.75m;

        // Act
        var result = Item.Create(name, description, basePrice, volatility);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be(name);
        result.Data!.Description.Should().Be(description);
        result.Data!.BasePrice.Should().Be(basePrice);
        result.Data!.Volatility.Should().Be(volatility);
    }

    [Fact]
    public void Create_ErrorMessage_NameEmpty()
    {
        var result = Item.Create(string.Empty, "Valid description", 10m, 0.2m);
        result.Error!.Message.Should().Contain("empty");
    }

    [Fact]
    public void Create_ErrorMessage_DescriptionEmpty()
    {
        var result = Item.Create("Valid name", string.Empty, 10m, 0.2m);
        result.Error!.Message.Should().Contain("empty");
    }

    [Fact]
    public void Create_ErrorMessage_BasePriceInvalid()
    {
        var result = Item.Create("Valid name", "Valid description", 0m, 0.2m);
        result.Error!.Message.Should().Contain("greater than zero");
    }

    [Fact]
    public void Create_ErrorMessage_VolatilityInvalid()
    {
        var result = Item.Create("Valid name", "Valid description", 10m, 1.5m);
        result.Error!.Message.Should().Contain("volatility");
    }

    [Fact]
    public void Create_WithMiddleVolatility_ReturnsSuccess()
    {
        var result = Item.Create("Item", "Description", 100m, 0.5m);
        result.IsSuccess.Should().BeTrue();
        result.Data!.Volatility.Should().Be(0.5m);
    }

    [Fact]
    public void Create_WithDecimalBasePrice_PreservesValue()
    {
        var basePrice = 123.45m;
        var result = Item.Create("Item", "Description", basePrice, 0.2m);
        result.IsSuccess.Should().BeTrue();
        result.Data!.BasePrice.Should().Be(basePrice);
    }

    [Fact]
    public void Create_WithLongStringValues_Succeeds()
    {
        var longName = new string('A', 500);
        var longDescription = new string('B', 5000);
        var result = Item.Create(longName, longDescription, 100m, 0.5m);
        result.IsSuccess.Should().BeTrue();
        result.Data!.Name.Should().Be(longName);
        result.Data!.Description.Should().Be(longDescription);
    }

    [Fact]
    public void Create_WithSpecialCharactersInName_Succeeds()
    {
        var specialName = "Sword™ of Power® [Legendary]";
        var result = Item.Create(specialName, "Description", 100m, 0.5m);
        result.IsSuccess.Should().BeTrue();
        result.Data!.Name.Should().Be(specialName);
    }

    [Fact]
    public void Create_WithSpecialCharactersInDescription_Succeeds()
    {
        var specialDescription = "Description with émojis 🎉 and spëcial çharacters!";
        var result = Item.Create("Item", specialDescription, 100m, 0.5m);
        result.IsSuccess.Should().BeTrue();
        result.Data!.Description.Should().Be(specialDescription);
    }

    [Fact]
    public void Create_ReturnType_IsResult()
    {
        var result = Item.Create("Item", "Description", 100m, 0.5m);
        result.Should().BeOfType(typeof(Result<Item>));
    }

    [Fact]
    public void Create_FailureReturnType_ContainsError()
    {
        var result = Item.Create(string.Empty, "Description", 100m, 0.5m);
        result.Should().BeOfType(typeof(Result<Item>));
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public void Create_SuccessReturnType_ContainsData()
    {
        var result = Item.Create("Item", "Description", 100m, 0.5m);
        result.Should().BeOfType(typeof(Result<Item>));
        result.Data.Should().NotBeNull();
    }

    [Fact]
    public void Create_WithDecimalVolatility_PreservesValue()
    {
        var volatility = 0.33m;
        var result = Item.Create("Item", "Description", 100m, volatility);
        result.IsSuccess.Should().BeTrue();
        result.Data!.Volatility.Should().Be(volatility);
    }

    [Fact]
    public void Create_BoundaryVolatility_ZeroBoundary()
    {
        var result = Item.Create("Item", "Description", 100m, 0m);
        result.IsSuccess.Should().BeTrue();
        result.Data!.Volatility.Should().Be(0m);
    }

    [Fact]
    public void Create_BoundaryVolatility_OneBoundary()
    {
        var result = Item.Create("Item", "Description", 100m, 1m);
        result.IsSuccess.Should().BeTrue();
        result.Data!.Volatility.Should().Be(1m);
    }

    [Fact]
    public void Create_JustAboveVolatilityOne_ReturnsFailure()
    {
        var result = Item.Create("Item", "Description", 100m, 1.0001m);
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Item.Volatility.Invalid");
    }

    [Fact]
    public void Create_JustBelowVolatilityZero_ReturnsFailure()
    {
        var result = Item.Create("Item", "Description", 100m, -0.0001m);
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Item.Volatility.Invalid");
    }
}
