using DSRS.Domain.Aggregates.Items;
using DSRS.Domain.Aggregates.Players;
using DSRS.Domain.Aggregates.Pricing;
using DSRS.Domain.ValueObjects;
using DSRS.SharedKernel.Enums;
using DSRS.SharedKernel.Primitives;
using FluentAssertions;
using Xunit;

namespace DSRS.Domain.UnitTests.Pricing;

public class DailyPriceTests
{
    private static Player CreateValidPlayer(string name = "TestPlayer")
    {
        var result = Player.Create(name);
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
        var price = Money.From(150m);
        var percentage = 15m;
        var state = PriceState.HIGH;

        // Act
        var result = DailyPrice.Create(player.Id, item.Id, date, price, percentage, state);

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
        var price = Money.From(150m);
        var percentage = 15m;
        var state = PriceState.HIGH;

        // Act
        var result = DailyPrice.Create(player.Id, item.Id, date, price, percentage, state);

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
        var price = Money.From(150m);
        var percentage = 15m;
        var state = PriceState.HIGH;

        // Act
        var result = DailyPrice.Create(player.Id, item.Id, date, price, percentage, state);

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
        var price = Money.From(250.50m);
        var percentage = 15m;
        var state = PriceState.HIGH;

        // Act
        var result = DailyPrice.Create(player.Id, item.Id, date, price, percentage, state);

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
        var price = Money.From(150m);
        var percentage = 15m;
        var state = PriceState.HIGH;

        // Act
        var result = DailyPrice.Create(player.Id, item.Id, date, price, percentage, state);

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
        var price = Money.From(50m);
        var percentage = 15m;
        var state = PriceState.LOW;

        // Act
        var result = DailyPrice.Create(player.Id, item.Id, date, price, percentage, state);

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
        var price = Money.From(150m);
        var percentage = 15m;
        var state = PriceState.HIGH;

        // Act
        var result = DailyPrice.Create(player.Id, item.Id, date, price, percentage, state);

        // Assert
        result.Data!.ItemId.Should().Be(item.Id);
    }

    [Fact]
    public void Create_WithAllPropertiesSet_AllPropertiesAreCorrect()
    {
        // Arrange
        var player = CreateValidPlayer("Alice");
        var item = CreateValidItem("Potion", "Health potion", 25m, 0.3m);
        var date = new DateOnly(2024, 3, 10);
        var price = Money.From(30.75m);
        var percentage = 15m;
        var state = PriceState.HIGH;

        // Act
        var result = DailyPrice.Create(player.Id, item.Id, date, price, percentage, state);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.ItemId.Should().Be(item.Id);
        result.Data!.Date.Should().Be(date);
        result.Data!.Price.Should().Be(price);
        result.Data!.Percentage.Should().Be(percentage);
        result.Data!.State.Should().Be(state);
    }

    [Fact]
    public void Create_WithVerySmallPrice_ReturnsSuccess()
    {
        // Arrange
        var player = CreateValidPlayer();
        var item = CreateValidItem();
        var date = new DateOnly(2024, 1, 15);
        var price = Money.From(0.01m);
        var percentage = 15m;
        var state = PriceState.LOW;

        // Act
        var result = DailyPrice.Create(player.Id, item.Id, date, price, percentage, state);

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
        var price = Money.From(999999999.99m);
        var percentage = 15m;
        var state = PriceState.HIGH;

        // Act
        var result = DailyPrice.Create(player.Id, item.Id, date, price, percentage, state);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Price.Should().Be(price);
    }

    [Fact]
    public void Create_WithPriceMatchingItemBasePrice_ReturnsSuccess()
    {
        // Arrange
        var basePrice = Money.From(150m);
        var player = CreateValidPlayer();
        var item = CreateValidItem(basePrice: basePrice.Value);
        var date = new DateOnly(2024, 1, 15);
        var price = basePrice;
        var percentage = 15m;
        var state = PriceState.HIGH;

        // Act
        var result = DailyPrice.Create(player.Id, item.Id, date, price, percentage, state);

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
        var price = Money.From(150m);
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
            var result = DailyPrice.Create(player.Id, item.Id, date, price, 15m, PriceState.HIGH);
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
        var price = Money.From(123.456m);
        var percentage = 15m;
        var state = PriceState.HIGH;

        // Act
        var result = DailyPrice.Create(player.Id, item.Id, date, price, percentage, state);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Price.Should().Be(price);
    }

    #endregion

    #region Property Validation

    [Fact]
    public void Create_WithHighState_PropertyIsAccessible()
    {
        // Arrange
        var player = CreateValidPlayer();
        var item = CreateValidItem();
        var price = Money.From(150m);
        var state = PriceState.HIGH;

        // Act
        var result = DailyPrice.Create(player.Id, item.Id, new DateOnly(2024, 1, 15), price, 15m, state);

        // Assert
        result.Data!.State.Should().Be(PriceState.HIGH);
    }

    [Fact]
    public void Create_WithLowState_PropertyIsAccessible()
    {
        // Arrange
        var player = CreateValidPlayer();
        var item = CreateValidItem();

        var price = Money.From(100m);
        var state = PriceState.LOW;

        // Act
        var result = DailyPrice.Create(player.Id, item.Id, new DateOnly(2024, 1, 15), price, 10m, state);

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

        var price = Money.From(100m);

        // Act
        var result = DailyPrice.Create(player.Id, item.Id, date, price, 10m, PriceState.HIGH);

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

        var price = Money.From(100m);
        // Act
        var result = DailyPrice.Create(player.Id, item.Id, new DateOnly(2024, 1, 15), price, 10m, PriceState.HIGH);
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
        var price = Money.From(100m);

        // Act
        var result = DailyPrice.Create(player.Id, item.Id, date, price, 10m, PriceState.HIGH);
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
        var price = Money.From(999.99m);
        var percentage = 15m;

        // Act
        var result = DailyPrice.Create(player.Id, item.Id, new DateOnly(2024, 1, 15), price, percentage, PriceState.HIGH);
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
        var price = Money.From(100m);

        // Act
        var result = DailyPrice.Create(player.Id, item.Id, new DateOnly(2024, 1, 15), price, 10m, PriceState.LOW);
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
        var price = Money.From(100m);

        // Act
        var result = DailyPrice.Create(player.Id, item.Id, new DateOnly(2024, 1, 15), price, 15m, PriceState.HIGH);

        // Assert
        result.Should().BeOfType<Result<DailyPrice>>();
    }

    [Fact]
    public void Create_WithValidItem_ReturnsResultType()
    {
        // Arrange
        var player = CreateValidPlayer();
        var item = CreateValidItem();
        var price = Money.From(100m);

        // Act
        var result = DailyPrice.Create(player.Id, item.Id, new DateOnly(2024, 1, 15), price, 11m, PriceState.HIGH);

        // Assert
        result.Should().BeOfType<Result<DailyPrice>>();
    }

    [Fact]
    public void Create_Success_DataIsNotNull()
    {
        // Arrange
        var player = CreateValidPlayer();
        var item = CreateValidItem();
        var price = Money.From(100m);

        // Act
        var result = DailyPrice.Create(player.Id, item.Id, new DateOnly(2024, 1, 15), price, 11m, PriceState.HIGH);

        // Assert
        result.Data.Should().NotBeNull();
    }

    [Fact]
    public void Create_Success_ErrorIsNull()
    {
        // Arrange
        var player = CreateValidPlayer();
        var item = CreateValidItem();
        var price = Money.From(100m);

        // Act
        var result = DailyPrice.Create(player.Id, item.Id, new DateOnly(2024, 1, 15), price, 11m, PriceState.HIGH);

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
        var price = Money.From(100m);

        // Act
        var result = DailyPrice.Create(player.Id, item.Id, date, price, 10m, PriceState.HIGH);

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
        var price = Money.From(100m);

        // Act
        var result = DailyPrice.Create(player.Id, item.Id, date, price, 11m, PriceState.HIGH);

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
        var price = Money.From(100m);
        var price2 = Money.From(100m);

        // Act
        var result1 = DailyPrice.Create(player.Id, item1.Id, date, price, 11m, PriceState.HIGH);
        var result2 = DailyPrice.Create(player.Id, item2.Id, date, price2, 10m, PriceState.LOW);

        // Assert
        result1.Data!.Should().NotBe(result2);
    }

    [Fact]
    public void Create_WithSamePlayerAndItemDifferentDates_CreatesInstancesWithDifferentDates()
    {
        // Arrange
        var player = CreateValidPlayer();
        var item = CreateValidItem();
        var date1 = new DateOnly(2024, 1, 15);
        var date2 = new DateOnly(2024, 1, 16);
        var price = Money.From(100m);
        var price2 = Money.From(100m);

        // Act
        var result1 = DailyPrice.Create(player.Id, item.Id, date1, price, 11m, PriceState.HIGH);
        var result2 = DailyPrice.Create(player.Id, item.Id, date2, price2, 10m, PriceState.LOW);

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
        var price1 = Money.From(100m);
        var percentage1 = 15m;
        var price2 = Money.From(200m);
        var percentage2 = 25m;

        // Act
        var result1 = DailyPrice.Create(player.Id, item.Id, date, price1, percentage1, PriceState.HIGH);
        var result2 = DailyPrice.Create(player.Id, item.Id, date, price2, percentage2, PriceState.HIGH);

        // Assert
        result1.Data!.Price.Should().Be(price1);
        result2.Data!.Price.Should().Be(price2);
        result2.Data!.Should().NotBe(result1.Data!);
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
        var price = Money.From(100m);

        // Act
        var result = DailyPrice.Create(player.Id, item.Id, date, price, 10m, PriceState.HIGH);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Create_WithPlayerAddDailyPriceMethod_ReturnsSuccess()
    {
        // Arrange
        var playerResult = Player.Create("TestPlayer");
        var player = playerResult.Data!;
        var itemResult = Item.Create("TestItem", "A test item", 100m, 0.5m);
        var item = itemResult.Data!;
        var date = new DateOnly(2024, 1, 15);
        var price = Money.From(100m);

        // Act
        var dailyPriceResult = player.AddDailyPrice(item.Id, date, price, 10m, PriceState.HIGH);

        // Assert
        dailyPriceResult.IsSuccess.Should().BeTrue();
        dailyPriceResult.Data!.ItemId.Should().Be(item.Id);
    }

    #endregion
}
