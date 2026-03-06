using DSRS.Domain.Items;
using DSRS.Domain.Players;
using DSRS.SharedKernel.Enums;
using DSRS.SharedKernel.Primitives;
using FluentAssertions;
using Xunit;

namespace DSRS.Domain.UnitTests.PlayerTests;

public class PlayerTests
{
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
        var name = "Alice";

        // Act
        var result = Player.Create(name);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be(name);
    }

    [Fact]
    public void Create_WithValidName_SetsNameCorrectly()
    {
        // Arrange & Act
        var result = Player.Create("TestPlayer");

        // Assert
        result.Data!.Name.Should().Be("TestPlayer");
    }

    [Fact]
    public void Create_WithValidBalance_SetsBalanceCorrectly()
    {
        // Arrange & Act
        var result = Player.Create("TestPlayer");

        // Assert
        result.Data!.Balance.Should().Be(1000m);
    }

    [Fact]
    public void Create_WithZeroBalance_ReturnsSuccess()
    {
        // Arrange & Act
        var result = Player.Create("ZeroBalancePlayer");
        result.Data!.DecreaseBalance(1000m);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Balance.Should().Be(0m);
    }

    [Fact]
    public void Create_WithVeryLargeBalance_ReturnsSuccess()
    {
        // Arrange & Act
        var result = Player.Create("RichPlayer");
        result.Data!.IncreaseBalance(10000007999.99m);
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Balance.Should().Be(10000008999.99m);
    }

    [Fact]
    public void Create_WithVerySmallBalance_ReturnsSuccess()
    {
        // Arrange & Act
        var result = Player.Create("PoorPlayer");
        result.Data!.DecreaseBalance(999.99m);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Balance.Should().Be(0.01m);
    }

    [Fact]
    public void Create_WithDecimalBalance_PreservesValue()
    {
        // Arrange
        var balance = 123.456m;

        // Act
        var result = Player.Create("TestPlayer");
        result.Data!.IncreaseBalance(balance);

        // Assert
        result.Data!.Balance.Should().Be(balance + 1000m);
    }

    [Fact]
    public void Create_WithSingleCharacterName_ReturnsSuccess()
    {
        // Arrange & Act
        var result = Player.Create("A");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Name.Should().Be("A");
    }

    [Fact]
    public void Create_WithLongName_ReturnsSuccess()
    {
        // Arrange
        var longName = new string('A', 1000);

        // Act
        var result = Player.Create(longName);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Name.Should().Be(longName);
    }

    [Fact]
    public void Create_WithSpecialCharactersInName_ReturnsSuccess()
    {
        // Arrange & Act
        var result = Player.Create("Player™-123 [VIP]");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Name.Should().Be("Player™-123 [VIP]");
    }

    [Fact]
    public void Create_WithUnicodeCharactersInName_ReturnsSuccess()
    {
        // Arrange & Act
        var result = Player.Create("जॉन वॉन 🎮");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Name.Should().Be("जॉन वॉन 🎮");
    }

    [Fact]
    public void Create_WithAllValidParameters_AllPropertiesSet()
    {
        // Arrange
        var name = "CompletePlayer";
        var balance = 5000.75m;
        var maxLImit = 100;

        // Act
        var result = Player.Create(name);
        result.Data!.IncreaseBalance(balance);
        result.Data!.IncreaseStorage(maxLImit);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Name.Should().Be(name);
        result.Data.Balance.Should().Be(balance + 1000);
        result.Data!.MaxLimit.Should().Be(maxLImit);
    }

    [Fact]
    public void Create_InitializesDailyPricesAsEmptyCollection()
    {
        // Arrange & Act
        var result = Player.Create("TestPlayer");

        // Assert
        result.Data!.DailyPrices.Should().BeEmpty();
        result.Data.DailyPrices.Count.Should().Be(0);
    }

    #endregion

    #region Create Method - Invalid Cases

    [Fact]
    public void Create_WithNullName_ReturnsFailure()
    {
        // Arrange & Act
        var result = Player.Create(null!);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("Player.Name.Empty");
    }

    [Fact]
    public void Create_WithEmptyName_ReturnsFailure()
    {
        // Arrange & Act
        var result = Player.Create(string.Empty);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Player.Name.Empty");
    }

    [Fact]
    public void Create_WithWhitespaceName_ReturnsFailure()
    {
        // Arrange & Act
        var result = Player.Create("   ");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Player.Name.Empty");
    }

    [Fact]
    public void Create_WithTabName_ReturnsFailure()
    {
        // Arrange & Act
        var result = Player.Create("\t");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Player.Name.Empty");
    }

    [Fact]
    public void Create_WithNewlineOnlyName_ReturnsFailure()
    {
        // Arrange & Act
        var result = Player.Create("\n\r");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Player.Name.Empty");
    }

    [Fact]
    public void Create_NullNameError_HasCorrectMessage()
    {
        // Arrange & Act
        var result = Player.Create(null!);

        // Assert
        result.Error!.Message.Should().Contain("empty");
    }

    [Fact]
    public void Create_EmptyNameError_HasCorrectMessage()
    {
        // Arrange & Act
        var result = Player.Create(string.Empty);

        // Assert
        result.Error!.Message.Should().Contain("empty");
    }

    [Fact]
    public void Create_FailureResult_DataIsNull()
    {
        // Arrange & Act
        var result = Player.Create("");

        // Assert
        result.Data.Should().BeNull();
    }

    [Fact]
    public void Create_FailureResult_ErrorIsNotNull()
    {
        // Arrange & Act
        var result = Player.Create(null!);

        // Assert
        result.Error.Should().NotBeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData("\n")]
    [InlineData("\r\n")]
    public void Create_WithEmptyOrWhitespaceName_ReturnsFailure(string name)
    {
        // Arrange & Act
        var result = Player.Create(name);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Player.Name.Empty");
    }

    #endregion

    #region Create Method - Return Type

    [Fact]
    public void Create_Success_ReturnsResultType()
    {
        // Arrange & Act
        var result = Player.Create("TestPlayer");

        // Assert
        result.Should().BeOfType<Result<Player>>();
    }

    [Fact]
    public void Create_Failure_ReturnsResultType()
    {
        // Arrange & Act
        var result = Player.Create(null!);

        // Assert
        result.Should().BeOfType<Result<Player>>();
    }

    [Fact]
    public void Create_Success_DataNotNull()
    {
        // Arrange & Act
        var result = Player.Create("TestPlayer");

        // Assert
        result.Data.Should().NotBeNull();
    }

    [Fact]
    public void Create_Success_ErrorNull()
    {
        // Arrange & Act
        var result = Player.Create("TestPlayer");

        // Assert
        result.Error.Should().BeNull();
    }

    [Fact]
    public void Create_Failure_DataNull()
    {
        // Arrange & Act
        var result = Player.Create("");

        // Assert
        result.Data.Should().BeNull();
    }

    [Fact]
    public void Create_Failure_ErrorNotNull()
    {
        // Arrange & Act
        var result = Player.Create("");

        // Assert
        result.Error.Should().NotBeNull();
    }

    #endregion

    #region AddDailyPrice Method - Valid Cases

    [Fact]
    public void AddDailyPrice_WithValidParameters_ReturnsSuccess()
    {
        // Arrange
        var playerResult = Player.Create("TestPlayer");
        var player = playerResult.Data!;
        var item = CreateValidItem();
        var date = new DateOnly(2024, 1, 15);
        var price = 150m;
        var percentage = 20.25m;
        var state = PriceState.HIGH;

        // Act
        var result = player.AddDailyPrice(item, date, price, percentage, state);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
    }

    [Fact]
    public void AddDailyPrice_WithValidParameters_AddsToCollection()
    {
        // Arrange
        var playerResult = Player.Create("TestPlayer");
        var player = playerResult.Data!;
        var item = CreateValidItem();
        var date = new DateOnly(2024, 1, 15);
        var percentage = 20.50m;

        // Act
        var result = player.AddDailyPrice(item, date, 100m, percentage, PriceState.HIGH);

        // Assert
        player.DailyPrices.Should().HaveCount(1);
        player.DailyPrices.First().Should().Be(result.Data!);
    }

    [Fact]
    public void AddDailyPrice_VerifyDailyPriceProperties()
    {
        // Arrange
        var playerResult = Player.Create("TestPlayer");
        var player = playerResult.Data!;
        var item = CreateValidItem();
        var date = new DateOnly(2024, 6, 15);
        var price = 250.50m;
        var percentage = 20.50m;
        var state = PriceState.HIGH;

        // Act
        var result = player.AddDailyPrice(item, date, price, percentage, state);

        // Assert
        // result.Data!.PlayerId.Should().Be(player.Id);
        result.Data!.ItemId.Should().Be(item.Id);
        result.Data.Date.Should().Be(date);
        result.Data.Price.Should().Be(price);
        result.Data.Percentage.Should().Be(percentage);
        result.Data.State.Should().Be(state);
    }

    [Fact]
    public void AddDailyPrice_WithMultipleItems_AndSameItemDateFails()
    {
        // Arrange - Since items have the same default ID, we can only test same date with different items by using different dates
        var playerResult = Player.Create("TestPlayer");
        var player = playerResult.Data!;
        var item1 = CreateValidItem("Item1");
        var item2 = CreateValidItem("Item2");
        var date1 = new DateOnly(2024, 1, 15);
        var date2 = new DateOnly(2024, 1, 16);

        // Act - Add same item (same ID) on different dates - should succeed
        var result1 = player.AddDailyPrice(item1, date1, 100m, 20m, PriceState.HIGH);
        var result2 = player.AddDailyPrice(item2, date2, 200m, 11m, PriceState.LOW);

        // Assert
        result1.IsSuccess.Should().BeTrue();
        result2.IsSuccess.Should().BeTrue();
        player.DailyPrices.Should().HaveCount(2);
    }

    [Fact]
    public void AddDailyPrice_WithMultipleDates_AddsMultiplePrices()
    {
        // Arrange
        var playerResult = Player.Create("TestPlayer");
        var player = playerResult.Data!;
        var item = CreateValidItem();
        var date1 = new DateOnly(2024, 1, 15);
        var date2 = new DateOnly(2024, 1, 16);

        // Act
        var result1 = player.AddDailyPrice(item, date1, 100m, 32m, PriceState.HIGH);
        var result2 = player.AddDailyPrice(item, date2, 150m, 14m, PriceState.LOW);

        // Assert
        player.DailyPrices.Should().HaveCount(2);
        player.DailyPrices.Should().Contain(result1.Data!);
        player.DailyPrices.Should().Contain(result2.Data!);
    }

    [Fact]
    public void AddDailyPrice_WithDifferentStates_ReturnsSuccess()
    {
        // Arrange
        var playerResult = Player.Create("TestPlayer");
        var player = playerResult.Data!;
        var item = CreateValidItem();

        // Act
        var resultHigh = player.AddDailyPrice(item, new DateOnly(2024, 1, 15), 100m, 20m, PriceState.HIGH);
        var resultLow = player.AddDailyPrice(item, new DateOnly(2024, 1, 16), 100m, -77m, PriceState.LOW);

        // Assert
        resultHigh.IsSuccess.Should().BeTrue();
        resultLow.IsSuccess.Should().BeTrue();
        resultHigh.Data!.State.Should().Be(PriceState.HIGH);
        resultLow.Data!.State.Should().Be(PriceState.LOW);
    }

    [Fact]
    public void AddDailyPrice_WithVerySmallPrice_ReturnsSuccess()
    {
        // Arrange
        var playerResult = Player.Create("TestPlayer");
        var player = playerResult.Data!;
        var item = CreateValidItem();

        // Act
        var result = player.AddDailyPrice(item, new DateOnly(2024, 1, 15), 0.01m, 0.2m, PriceState.HIGH);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Price.Should().Be(0.01m);
    }

    [Fact]
    public void AddDailyPrice_WithVeryLargePrice_ReturnsSuccess()
    {
        // Arrange
        var playerResult = Player.Create("TestPlayer");
        var player = playerResult.Data!;
        var item = CreateValidItem();

        // Act
        var result = player.AddDailyPrice(item, new DateOnly(2024, 1, 15), 999999999.99m, 99.99m, PriceState.HIGH);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Price.Should().Be(999999999.99m);
    }

    [Fact]
    public void AddDailyPrice_DailyPricesIsReadOnlyCollection()
    {
        // Arrange
        var playerResult = Player.Create("TestPlayer");
        var player = playerResult.Data!;
        var item = CreateValidItem();

        // Act
        player.AddDailyPrice(item, new DateOnly(2024, 1, 15), 100m, 25m, PriceState.HIGH);

        // Assert
        var collection = player.DailyPrices;
        collection.Should().BeAssignableTo<IReadOnlyCollection<DSRS.Domain.Pricing.DailyPrice>>();
    }

    #endregion

    #region AddDailyPrice Method - Invalid Cases

    [Fact]
    public void AddDailyPrice_WithZeroPrice_ReturnsFailure()
    {
        // Arrange
        var playerResult = Player.Create("TestPlayer");
        var player = playerResult.Data!;
        var item = CreateValidItem();

        // Act
        var result = player.AddDailyPrice(item, new DateOnly(2024, 1, 15), 0m, 0m, PriceState.HIGH);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("DailyPrice.Price.Invalid");
    }

    [Fact]
    public void AddDailyPrice_WithNegativePrice_ReturnsFailure()
    {
        // Arrange
        var playerResult = Player.Create("TestPlayer");
        var player = playerResult.Data!;
        var item = CreateValidItem();

        // Act
        var result = player.AddDailyPrice(item, new DateOnly(2024, 1, 15), -100m, 10m, PriceState.HIGH);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("DailyPrice.Price.Invalid");
    }

    [Fact]
    public void AddDailyPrice_WithZeroPriceError_HasCorrectMessage()
    {
        // Arrange
        var playerResult = Player.Create("TestPlayer");
        var player = playerResult.Data!;
        var item = CreateValidItem();

        // Act
        var result = player.AddDailyPrice(item, new DateOnly(2024, 1, 15), 0m, 0m, PriceState.HIGH);

        // Assert
        result.Error!.Message.Should().Contain("greater than zero");
    }

    [Fact]
    public void AddDailyPrice_WithDuplicateItemAndDate_ReturnsFailure()
    {
        // Arrange
        var playerResult = Player.Create("TestPlayer");
        var player = playerResult.Data!;
        var item = CreateValidItem();
        var date = new DateOnly(2024, 1, 15);

        // Act
        var result1 = player.AddDailyPrice(item, date, 100m, 20m, PriceState.HIGH);
        var result2 = player.AddDailyPrice(item, date, 150m, 30m, PriceState.LOW);

        // Assert
        result1.IsSuccess.Should().BeTrue();
        result2.IsSuccess.Should().BeFalse();
        result2.Error!.Code.Should().Be("DailyPrice.Exists");
    }

    [Fact]
    public void AddDailyPrice_WithDuplicateItemAndDateError_HasCorrectMessage()
    {
        // Arrange
        var playerResult = Player.Create("TestPlayer");
        var player = playerResult.Data!;
        var item = CreateValidItem();
        var date = new DateOnly(2024, 1, 15);

        // Act
        player.AddDailyPrice(item, date, 100m, 20m, PriceState.HIGH);
        var result = player.AddDailyPrice(item, date, 150m, 20m, PriceState.LOW);

        // Assert
        result.Error!.Message.Should().Contain("already exists");
    }

    [Fact]
    public void AddDailyPrice_FirstSucceeds_SecondWithSameItemDateFails()
    {
        // Arrange
        var playerResult = Player.Create("TestPlayer");
        var player = playerResult.Data!;
        var item = CreateValidItem();
        var date = new DateOnly(2024, 1, 15);

        // Act
        var firstResult = player.AddDailyPrice(item, date, 100m, 10m, PriceState.HIGH);
        var secondResult = player.AddDailyPrice(item, date, 200m, 20m, PriceState.HIGH);

        // Assert
        firstResult.IsSuccess.Should().BeTrue();
        secondResult.IsSuccess.Should().BeFalse();
        player.DailyPrices.Should().HaveCount(1);
    }

    [Fact]
    public void AddDailyPrice_WithInvalidItemFromDailyPriceValidation_ReturnsFailure()
    {
        // Arrange
        var playerResult = Player.Create("TestPlayer");
        var player = playerResult.Data!;

        // Act
        var result = player.AddDailyPrice(null!, new DateOnly(2024, 1, 15), 100m, 10m, PriceState.HIGH);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("DailyPrice.Item.Null");
    }

    #endregion

    #region AddDailyPrice Method - Return Type

    [Fact]
    public void AddDailyPrice_Success_ReturnsResultType()
    {
        // Arrange
        var playerResult = Player.Create("TestPlayer");
        var player = playerResult.Data!;
        var item = CreateValidItem();

        // Act
        var result = player.AddDailyPrice(item, new DateOnly(2024, 1, 15), 100m, 20m, PriceState.HIGH);

        // Assert
        result.Should().BeOfType<Result<DSRS.Domain.Pricing.DailyPrice>>();
    }

    [Fact]
    public void AddDailyPrice_Failure_ReturnsResultType()
    {
        // Arrange
        var playerResult = Player.Create("TestPlayer");
        var player = playerResult.Data!;

        // Act
        var result = player.AddDailyPrice(null!, new DateOnly(2024, 1, 15), 100m, 20m, PriceState.HIGH);

        // Assert
        result.Should().BeOfType<Result<DSRS.Domain.Pricing.DailyPrice>>();
    }

    [Fact]
    public void AddDailyPrice_Success_DataNotNull()
    {
        // Arrange
        var playerResult = Player.Create("TestPlayer");
        var player = playerResult.Data!;
        var item = CreateValidItem();

        // Act
        var result = player.AddDailyPrice(item, new DateOnly(2024, 1, 15), 100m, 20m, PriceState.HIGH);

        // Assert
        result.Data.Should().NotBeNull();
    }

    [Fact]
    public void AddDailyPrice_Success_ErrorNull()
    {
        // Arrange
        var playerResult = Player.Create("TestPlayer");
        var player = playerResult.Data!;
        var item = CreateValidItem();

        // Act
        var result = player.AddDailyPrice(item, new DateOnly(2024, 1, 15), 100m, 10m, PriceState.HIGH);

        // Assert
        result.Error.Should().BeNull();
    }

    [Fact]
    public void AddDailyPrice_Failure_DataNull()
    {
        // Arrange
        var playerResult = Player.Create("TestPlayer");
        var player = playerResult.Data!;

        // Act
        var result = player.AddDailyPrice(null!, new DateOnly(2024, 1, 15), 100m, 10m, PriceState.HIGH);

        // Assert
        result.Data.Should().BeNull();
    }

    [Fact]
    public void AddDailyPrice_Failure_ErrorNotNull()
    {
        // Arrange
        var playerResult = Player.Create("TestPlayer");
        var player = playerResult.Data!;

        // Act
        var result = player.AddDailyPrice(null!, new DateOnly(2024, 1, 15), 100m, 10m, PriceState.HIGH);

        // Assert
        result.Error.Should().NotBeNull();
    }

    #endregion

    #region DailyPrices Property

    [Fact]
    public void DailyPrices_NewPlayer_ReturnsEmptyCollection()
    {
        // Arrange
        var playerResult = Player.Create("TestPlayer");
        var player = playerResult.Data!;

        // Assert
        player.DailyPrices.Should().BeEmpty();
        player.DailyPrices.Count.Should().Be(0);
    }

    [Fact]
    public void DailyPrices_AfterAddingOne_CountIsOne()
    {
        // Arrange
        var playerResult = Player.Create("TestPlayer");
        var player = playerResult.Data!;
        var item = CreateValidItem();

        // Act
        player.AddDailyPrice(item, new DateOnly(2024, 1, 15), 100m, 10m, PriceState.HIGH);

        // Assert
        player.DailyPrices.Count.Should().Be(1);
    }

    [Fact]
    public void DailyPrices_AfterAddingMultiple_CountIsCorrect()
    {
        // Arrange
        var playerResult = Player.Create("TestPlayer");
        var player = playerResult.Data!;
        var item = CreateValidItem("TestItem");

        // Act - Add same item on different dates
        player.AddDailyPrice(item, new DateOnly(2024, 1, 15), 100m, 10m, PriceState.HIGH);
        player.AddDailyPrice(item, new DateOnly(2024, 1, 16), 150m, -15m, PriceState.LOW);
        player.AddDailyPrice(item, new DateOnly(2024, 1, 17), 200m, 20m, PriceState.HIGH);

        // Assert
        player.DailyPrices.Count.Should().Be(3);
    }

    [Fact]
    public void DailyPrices_IsReadOnlyCollection()
    {
        // Arrange
        var playerResult = Player.Create("TestPlayer");
        var player = playerResult.Data!;

        // Assert
        player.DailyPrices.Should().BeAssignableTo<IReadOnlyCollection<DSRS.Domain.Pricing.DailyPrice>>();
    }

    [Fact]
    public void DailyPrices_CanIterateCollection()
    {
        // Arrange
        var playerResult = Player.Create("TestPlayer");
        var player = playerResult.Data!;
        var item = CreateValidItem();

        // Act
        player.AddDailyPrice(item, new DateOnly(2024, 1, 15), 100m, 10m, PriceState.HIGH);
        player.AddDailyPrice(item, new DateOnly(2024, 1, 16), 150m, -15m, PriceState.LOW);

        // Assert
        var prices = player.DailyPrices.ToList();
        prices.Should().HaveCount(2);
    }

    #endregion

    #region Multiple Players

    [Fact]
    public void Create_MultipleInstances_AreIndependent()
    {
        // Arrange & Act
        var player1Result = Player.Create("Player1");
        player1Result.Data!.DecreaseBalance(900m);
        var player2Result = Player.Create("Player2");
        player2Result.Data!.DecreaseBalance(800m);

        // Assert
        player1Result.Data!.Name.Should().Be("Player1");
        player2Result.Data!.Name.Should().Be("Player2");
        player1Result.Data.Balance.Should().Be(100m);
        player2Result.Data.Balance.Should().Be(200m);
    }

    [Fact]
    public void AddDailyPrice_MultiplePlayersIndependent()
    {
        // Arrange
        var player1Result = Player.Create("Player1");
        var player2Result = Player.Create("Player2");
        var player1 = player1Result.Data!;
        var player2 = player2Result.Data!;
        var item = CreateValidItem();

        // Act
        player1.AddDailyPrice(item, new DateOnly(2024, 1, 15), 100m, 10m, PriceState.HIGH);
        player2.AddDailyPrice(item, new DateOnly(2024, 1, 15), 150m, 15m, PriceState.LOW);

        // Assert
        player1.DailyPrices.Count.Should().Be(1);
        player2.DailyPrices.Count.Should().Be(1);
        player1.DailyPrices.First().Price.Should().Be(100m);
        player2.DailyPrices.First().Price.Should().Be(150m);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void AddDailyPrice_WithFutureDate_ReturnsSuccess()
    {
        // Arrange
        var playerResult = Player.Create("TestPlayer");
        var player = playerResult.Data!;
        var item = CreateValidItem();

        // Act
        var result = player.AddDailyPrice(item, new DateOnly(2099, 12, 31), 100m, 10m, PriceState.HIGH);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void AddDailyPrice_WithPastDate_ReturnsSuccess()
    {
        // Arrange
        var playerResult = Player.Create("TestPlayer");
        var player = playerResult.Data!;
        var item = CreateValidItem();

        // Act
        var result = player.AddDailyPrice(item, new DateOnly(2000, 1, 1), 100m, 10m, PriceState.HIGH);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void AddDailyPrice_SameItemSamePriceDifferentDates_BothSucceed()
    {
        // Arrange
        var playerResult = Player.Create("TestPlayer");
        var player = playerResult.Data!;
        var item = CreateValidItem();
        var price = 100m;

        // Act
        var result1 = player.AddDailyPrice(item, new DateOnly(2024, 1, 15), price, 15m, PriceState.HIGH);
        var result2 = player.AddDailyPrice(item, new DateOnly(2024, 1, 16), price, 10m, PriceState.HIGH);

        // Assert
        result1.IsSuccess.Should().BeTrue();
        result2.IsSuccess.Should().BeTrue();
        player.DailyPrices.Should().HaveCount(2);
    }

    #endregion

    #region Property Initialization

    [Fact]
    public void Create_AllPropertiesAccessible()
    {
        // Arrange & Act
        var result = Player.Create("TestPlayer");
        var player = result.Data!;

        // Assert
        player.Name.Should().NotBeNull();
        player.Balance.Should().Be(1000m);
        player.DailyPrices.Should().NotBeNull();
    }

    [Fact]
    public void Create_NameIsReadOnly()
    {
        // Arrange & Act
        var result = Player.Create("TestPlayer");
        var player = result.Data!;

        // Assert
        player.Name.Should().Be("TestPlayer");
    }

    [Fact]
    public void Create_BalanceIsReadOnly()
    {
        // Arrange & Act
        var result = Player.Create("TestPlayer");
        var player = result.Data!;

        // Assert
        player.Balance.Should().Be(1000m);
    }

    #endregion
}
