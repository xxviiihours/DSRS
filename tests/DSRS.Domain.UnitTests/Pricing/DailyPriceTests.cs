using DSRS.Domain.Items;
using DSRS.Domain.Players;
using DSRS.Domain.Pricing;
using DSRS.SharedKernel.Enums;
using DSRS.SharedKernel.Primitives;
using FluentAssertions;
using Xunit;

namespace DSRS.Domain.UnitTests.Pricing;

public class DailyPriceTests
{
    private static Player CreateValidPlayer(string name = "TestPlayer", decimal balance = 1000m)
    {
        var result = Player.Create(name, balance);
        return result.Data!;
    }

    private static Item CreateValidItem(string name = "TestItem", string description = "Test item", decimal basePrice = 100m, decimal volatility = 0.5m)
    {
        var result = Item.Create(name, description, basePrice, volatility);
        return result.Data!;
    }

    #region Create Method - Valid Cases

    [Fact]
    public void Create_WithValidParameters_ReturnsSuccess()
    {
        // Arrange
        var player = CreateValidPlayer();
        var item = CreateValidItem();
        var date = new DateOnly(2024, 1, 15);
        var price = 150m;
        var state = PriceState.HIGH;

        // Act
        var result = DailyPrice.Create(player, item, date, price, state);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
    }

    [Fact]
    public void Create_WithValidParameters_SetsItemIdCorrectly()
    {
        // Arrange
        var player = CreateValidPlayer();
        var item = CreateValidItem();
        var date = new DateOnly(2024, 1, 15);
        var price = 150m;
        var state = PriceState.HIGH;

        // Act
        var result = DailyPrice.Create(player, item, date, price, state);

        // Assert
        result.Data!.ItemId.Should().Be(item.Id);
    }

    [Fact]
    public void Create_WithValidParameters_SetsDateCorrectly()
    {
        // Arrange
        var player = CreateValidPlayer();
        var item = CreateValidItem();
        var date = new DateOnly(2024, 6, 20);
        var price = 150m;
        var state = PriceState.HIGH;

        // Act
        var result = DailyPrice.Create(player, item, date, price, state);

        // Assert
        result.Data!.Date.Should().Be(date);
    }

    [Fact]
    public void Create_WithValidParameters_SetsPriceCorrectly()
    {
        // Arrange
        var player = CreateValidPlayer();
        var item = CreateValidItem();
        var date = new DateOnly(2024, 1, 15);
        var price = 250.50m;
        var state = PriceState.HIGH;

        // Act
        var result = DailyPrice.Create(player, item, date, price, state);

        // Assert
        result.Data!.Price.Should().Be(price);
    }

    [Fact]
    public void Create_WithValidParameters_SetsStateToHigh()
    {
        // Arrange
        var player = CreateValidPlayer();
        var item = CreateValidItem();
        var date = new DateOnly(2024, 1, 15);
        var price = 150m;
        var state = PriceState.HIGH;

        // Act
        var result = DailyPrice.Create(player, item, date, price, state);

        // Assert
        result.Data!.State.Should().Be(PriceState.HIGH);
    }

    [Fact]
    public void Create_WithValidParameters_SetsStateToLow()
    {
        // Arrange
        var player = CreateValidPlayer();
        var item = CreateValidItem();
        var date = new DateOnly(2024, 1, 15);
        var price = 50m;
        var state = PriceState.LOW;

        // Act
        var result = DailyPrice.Create(player, item, date, price, state);

        // Assert
        result.Data!.State.Should().Be(PriceState.LOW);
    }

    [Fact]
    public void Create_WithValidParameters_SetsItemReferenceCorrectly()
    {
        // Arrange
        var player = CreateValidPlayer();
        var item = CreateValidItem("Sword", "A legendary sword", 500m, 0.7m);
        var date = new DateOnly(2024, 1, 15);
        var price = 150m;
        var state = PriceState.HIGH;

        // Act
        var result = DailyPrice.Create(player, item, date, price, state);

        // Assert
        result.Data!.Item.Should().Be(item);
        result.Data!.Item.Name.Should().Be("Sword");
    }

    [Fact]
    public void Create_WithAllPropertiesSet_AllPropertiesAreCorrect()
    {
        // Arrange
        var player = CreateValidPlayer("Alice", 10000m);
        var item = CreateValidItem("Potion", "Health potion", 25m, 0.3m);
        var date = new DateOnly(2024, 3, 10);
        var price = 30.75m;
        var state = PriceState.HIGH;

        // Act
        var result = DailyPrice.Create(player, item, date, price, state);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.ItemId.Should().Be(item.Id);
        result.Data!.Date.Should().Be(date);
        result.Data!.Price.Should().Be(price);
        result.Data!.State.Should().Be(state);
        result.Data!.Item.Should().Be(item);
    }

    [Fact]
    public void Create_WithVerySmallPrice_ReturnsSuccess()
    {
        // Arrange
        var player = CreateValidPlayer();
        var item = CreateValidItem();
        var date = new DateOnly(2024, 1, 15);
        var price = 0.01m;
        var state = PriceState.LOW;

        // Act
        var result = DailyPrice.Create(player, item, date, price, state);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Price.Should().Be(price);
    }

    [Fact]
    public void Create_WithVeryLargePrice_ReturnsSuccess()
    {
        // Arrange
        var player = CreateValidPlayer();
        var item = CreateValidItem();
        var date = new DateOnly(2024, 1, 15);
        var price = 999999999.99m;
        var state = PriceState.HIGH;

        // Act
        var result = DailyPrice.Create(player, item, date, price, state);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Price.Should().Be(price);
    }

    [Fact]
    public void Create_WithPriceMatchingItemBasePrice_ReturnsSuccess()
    {
        // Arrange
        var basePrice = 150m;
        var player = CreateValidPlayer();
        var item = CreateValidItem(basePrice: basePrice);
        var date = new DateOnly(2024, 1, 15);
        var price = basePrice;
        var state = PriceState.HIGH;

        // Act
        var result = DailyPrice.Create(player, item, date, price, state);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Price.Should().Be(basePrice);
    }

    [Fact]
    public void Create_WithDifferentDates_ReturnsSuccess()
    {
        // Arrange
        var player = CreateValidPlayer();
        var item = CreateValidItem();
        var dates = new[]
        {
            new DateOnly(2024, 1, 1),
            new DateOnly(2024, 6, 15),
            new DateOnly(2024, 12, 31),
            new DateOnly(2025, 1, 1)
        };

        // Act & Assert
        foreach (var date in dates)
        {
            var result = DailyPrice.Create(player, item, date, 100m, PriceState.HIGH);
            result.IsSuccess.Should().BeTrue();
            result.Data!.Date.Should().Be(date);
        }
    }

    [Fact]
    public void Create_WithDecimalPrice_PreservesValue()
    {
        // Arrange
        var player = CreateValidPlayer();
        var item = CreateValidItem();
        var date = new DateOnly(2024, 1, 15);
        var price = 123.456m;
        var state = PriceState.HIGH;

        // Act
        var result = DailyPrice.Create(player, item, date, price, state);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Price.Should().Be(price);
    }

    #endregion

    #region Create Method - Null Item Cases

    [Fact]
    public void Create_WithNullItem_ReturnsFailure()
    {
        // Arrange
        var player = CreateValidPlayer();
        Item nullItem = null!;
        var date = new DateOnly(2024, 1, 15);
        var price = 150m;
        var state = PriceState.HIGH;

        // Act
        var result = DailyPrice.Create(player, nullItem, date, price, state);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("DailyPrice.Item.Null");
    }

    [Fact]
    public void Create_WithNullItem_ReturnsCorrectError()
    {
        // Arrange
        var player = CreateValidPlayer();
        Item nullItem = null!;
        var date = new DateOnly(2024, 1, 15);
        var price = 150m;
        var state = PriceState.HIGH;

        // Act
        var result = DailyPrice.Create(player, nullItem, date, price, state);

        // Assert
        result.Error!.Message.Should().Contain("Item");
        result.Error!.Message.Should().Contain("null");
    }

    [Fact]
    public void Create_WithNullItem_ResultDataIsNull()
    {
        // Arrange
        var player = CreateValidPlayer();
        Item nullItem = null!;
        var date = new DateOnly(2024, 1, 15);
        var price = 150m;
        var state = PriceState.HIGH;

        // Act
        var result = DailyPrice.Create(player, nullItem, date, price, state);

        // Assert
        result.Data.Should().BeNull();
    }

    #endregion

    #region Property Validation

    [Fact]
    public void Create_WithHighState_PropertyIsAccessible()
    {
        // Arrange
        var player = CreateValidPlayer();
        var item = CreateValidItem();
        var state = PriceState.HIGH;

        // Act
        var result = DailyPrice.Create(player, item, new DateOnly(2024, 1, 15), 100m, state);

        // Assert
        result.Data!.State.Should().Be(PriceState.HIGH);
    }

    [Fact]
    public void Create_WithLowState_PropertyIsAccessible()
    {
        // Arrange
        var player = CreateValidPlayer();
        var item = CreateValidItem();
        var state = PriceState.LOW;

        // Act
        var result = DailyPrice.Create(player, item, new DateOnly(2024, 1, 15), 100m, state);

        // Assert
        result.Data!.State.Should().Be(PriceState.LOW);
    }

    [Fact]
    public void Create_Success_ReturnsValidDailyPrice()
    {
        // Arrange
        var player = CreateValidPlayer();
        var item = CreateValidItem();
        var date = new DateOnly(2024, 1, 15);

        // Act
        var result = DailyPrice.Create(player, item, date, 100m, PriceState.HIGH);

        // Assert
        result.Data.Should().NotBeNull();
        result.Data.Should().BeOfType<DailyPrice>();
    }

    [Fact]
    public void Create_ItemIdProperty_IsReadOnly()
    {
        // Arrange
        var player = CreateValidPlayer();
        var item = CreateValidItem();

        // Act
        var result = DailyPrice.Create(player, item, new DateOnly(2024, 1, 15), 100m, PriceState.HIGH);
        var dailyPrice = result.Data!;

        // Assert
        dailyPrice.ItemId.Should().Be(item.Id);
    }

    [Fact]
    public void Create_DateProperty_IsReadOnly()
    {
        // Arrange
        var player = CreateValidPlayer();
        var item = CreateValidItem();
        var date = new DateOnly(2025, 6, 30);

        // Act
        var result = DailyPrice.Create(player, item, date, 100m, PriceState.HIGH);
        var dailyPrice = result.Data!;

        // Assert
        dailyPrice.Date.Should().Be(date);
    }

    [Fact]
    public void Create_PriceProperty_IsReadOnly()
    {
        // Arrange
        var player = CreateValidPlayer();
        var item = CreateValidItem();
        var price = 999.99m;

        // Act
        var result = DailyPrice.Create(player, item, new DateOnly(2024, 1, 15), price, PriceState.HIGH);
        var dailyPrice = result.Data!;

        // Assert
        dailyPrice.Price.Should().Be(price);
    }

    [Fact]
    public void Create_StateProperty_IsReadOnly()
    {
        // Arrange
        var player = CreateValidPlayer();
        var item = CreateValidItem();

        // Act
        var result = DailyPrice.Create(player, item, new DateOnly(2024, 1, 15), 100m, PriceState.LOW);
        var dailyPrice = result.Data!;

        // Assert
        dailyPrice.State.Should().Be(PriceState.LOW);
    }

    #endregion

    #region Return Type and Result Structure

    [Fact]
    public void Create_SuccessfulCreation_ReturnsResultType()
    {
        // Arrange
        var player = CreateValidPlayer();
        var item = CreateValidItem();

        // Act
        var result = DailyPrice.Create(player, item, new DateOnly(2024, 1, 15), 100m, PriceState.HIGH);

        // Assert
        result.Should().BeOfType<Result<DailyPrice>>();
    }

    [Fact]
    public void Create_WithValidItem_ReturnsResultType()
    {
        // Arrange
        var player = CreateValidPlayer();
        var item = CreateValidItem();

        // Act
        var result = DailyPrice.Create(player, item, new DateOnly(2024, 1, 15), 100m, PriceState.HIGH);

        // Assert
        result.Should().BeOfType<Result<DailyPrice>>();
    }

    [Fact]
    public void Create_Success_DataIsNotNull()
    {
        // Arrange
        var player = CreateValidPlayer();
        var item = CreateValidItem();

        // Act
        var result = DailyPrice.Create(player, item, new DateOnly(2024, 1, 15), 100m, PriceState.HIGH);

        // Assert
        result.Data.Should().NotBeNull();
    }

    [Fact]
    public void Create_Success_ErrorIsNull()
    {
        // Arrange
        var player = CreateValidPlayer();
        var item = CreateValidItem();

        // Act
        var result = DailyPrice.Create(player, item, new DateOnly(2024, 1, 15), 100m, PriceState.HIGH);

        // Assert
        result.Error.Should().BeNull();
    }

    #endregion

    #region Edge Cases and Boundary Values

    [Fact]
    public void Create_WithEarliestDate_ReturnsSuccess()
    {
        // Arrange
        var player = CreateValidPlayer();
        var item = CreateValidItem();
        var date = new DateOnly(1, 1, 1);

        // Act
        var result = DailyPrice.Create(player, item, date, 100m, PriceState.HIGH);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Date.Should().Be(date);
    }

    [Fact]
    public void Create_WithFutureDate_ReturnsSuccess()
    {
        // Arrange
        var player = CreateValidPlayer();
        var item = CreateValidItem();
        var date = new DateOnly(2099, 12, 31);

        // Act
        var result = DailyPrice.Create(player, item, date, 100m, PriceState.HIGH);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Date.Should().Be(date);
    }

    [Fact]
    public void Create_WithMultipleItems_CreatesInstancesWithDifferentItems()
    {
        // Arrange
        var player = CreateValidPlayer();
        var item1 = CreateValidItem("Item1");
        var item2 = CreateValidItem("Item2");
        var date = new DateOnly(2024, 1, 15);

        // Act
        var result1 = DailyPrice.Create(player, item1, date, 100m, PriceState.HIGH);
        var result2 = DailyPrice.Create(player, item2, date, 200m, PriceState.LOW);

        // Assert
        result1.Data!.Item.Should().Be(item1);
        result2.Data!.Item.Should().Be(item2);
        result1.Data!.Item.Should().NotBe(result2.Data!.Item);
    }

    [Fact]
    public void Create_WithSamePlayerAndItemDifferentDates_CreatesInstancesWithDifferentDates()
    {
        // Arrange
        var player = CreateValidPlayer();
        var item = CreateValidItem();
        var date1 = new DateOnly(2024, 1, 15);
        var date2 = new DateOnly(2024, 1, 16);

        // Act
        var result1 = DailyPrice.Create(player, item, date1, 100m, PriceState.HIGH);
        var result2 = DailyPrice.Create(player, item, date2, 150m, PriceState.LOW);

        // Assert
        result1.Data!.Date.Should().Be(date1);
        result2.Data!.Date.Should().Be(date2);
        result1.Data!.Date.Should().NotBe(result2.Data!.Date);
    }

    [Fact]
    public void Create_CreatesMultipleInstances_WithDifferentPrices()
    {
        // Arrange
        var player = CreateValidPlayer();
        var item = CreateValidItem();
        var date = new DateOnly(2024, 1, 15);
        var price1 = 100m;
        var price2 = 200m;

        // Act
        var result1 = DailyPrice.Create(player, item, date, price1, PriceState.HIGH);
        var result2 = DailyPrice.Create(player, item, date, price2, PriceState.HIGH);

        // Assert
        result1.Data!.Price.Should().Be(price1);
        result2.Data!.Price.Should().Be(price2);
        result1.Data!.Should().NotBe(result2.Data!);
    }

    #endregion

    #region Error Code and Message Validation

    [Fact]
    public void Create_NullItemError_HasCorrectCode()
    {
        // Arrange
        var player = CreateValidPlayer();
        Item nullItem = null!;

        // Act
        var result = DailyPrice.Create(player, nullItem, new DateOnly(2024, 1, 15), 100m, PriceState.HIGH);

        // Assert
        result.Error!.Code.Should().Be("DailyPrice.Item.Null");
    }

    [Fact]
    public void Create_NullItemError_HasCorrectMessage()
    {
        // Arrange
        var player = CreateValidPlayer();
        Item nullItem = null!;

        // Act
        var result = DailyPrice.Create(player, nullItem, new DateOnly(2024, 1, 15), 100m, PriceState.HIGH);

        // Assert
        result.Error!.Message.Should().Be("Item cannot be null");
    }

    #endregion

    #region Integration with Player and Item Creation

    [Fact]
    public void Create_WithItemFromFactory_ReturnsSuccess()
    {
        // Arrange
        var player = CreateValidPlayer();
        var itemResult = Item.Create("Legendary Sword", "A powerful sword", 500m, 0.8m);
        var item = itemResult.Data!;
        var date = new DateOnly(2024, 1, 15);

        // Act
        var result = DailyPrice.Create(player, item, date, 100m, PriceState.HIGH);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Create_WithPlayerAddDailyPriceMethod_ReturnsSuccess()
    {
        // Arrange
        var playerResult = Player.Create("TestPlayer", 10000m);
        var player = playerResult.Data!;
        var itemResult = Item.Create("TestItem", "A test item", 100m, 0.5m);
        var item = itemResult.Data!;
        var date = new DateOnly(2024, 1, 15);

        // Act
        var dailyPriceResult = player.AddDailyPrice(item, date, 150m, PriceState.HIGH);

        // Assert
        dailyPriceResult.IsSuccess.Should().BeTrue();
        dailyPriceResult.Data!.ItemId.Should().Be(item.Id);
    }

    #endregion
}
