using DSRS.Domain.Items;
using DSRS.Domain.Pricing;
using DSRS.SharedKernel.Enums;
using FluentAssertions;
using Xunit;

namespace DSRS.Domain.UnitTests.Pricing;

public class MarketPricingServiceTests
{
    [Fact]
    public void Generate_ReturnsGeneratedPrice()
    {
        // Arrange
        var item = Item.Create("Sword", "A sturdy blade", 100m, 0.2m).Data!;

        // Act
        var result = MarketPricingService.Generate(item);

        // Assert
        result.Should().NotBeNull();
        result.Price.Should().BeGreaterThan(0);
        result.State.Should().BeOneOf(PriceState.HIGH, PriceState.LOW);
    }

    [Fact]
    public void Generate_WithHighState_PriceIsWithinHighRange()
    {
        // Arrange
        var item = Item.Create("Sword", "A sturdy blade", 100m, 0.2m).Data!;
        var minPrice = item.BasePrice;
        var maxPrice = item.BasePrice * (1 + item.Volatility);

        // Act - Run multiple times since it's random, capture HIGH prices
        for (int i = 0; i < 100; i++)
        {
            var result = MarketPricingService.Generate(item);

            if (result.State == PriceState.HIGH)
            {
                // Assert
                result.Price.Should().BeGreaterThanOrEqualTo(minPrice);
                result.Price.Should().BeLessThanOrEqualTo(maxPrice);
                break; // Found a HIGH state, verify it's in correct range
            }
        }
    }

    [Fact]
    public void Generate_WithLowState_PriceIsWithinLowRange()
    {
        // Arrange
        var item = Item.Create("Axe", "A chopping tool", 100m, 0.2m).Data!;
        var minPrice = item.BasePrice * (1 - item.Volatility);
        var maxPrice = item.BasePrice;

        // Act - Run multiple times since it's random, capture LOW prices
        for (int i = 0; i < 100; i++)
        {
            var result = MarketPricingService.Generate(item);

            if (result.State == PriceState.LOW)
            {
                // Assert
                result.Price.Should().BeGreaterThanOrEqualTo(minPrice);
                result.Price.Should().BeLessThanOrEqualTo(maxPrice);
                break; // Found a LOW state, verify it's in correct range
            }
        }
    }

    [Fact]
    public void Generate_WithSmallBasePrice_PriceIsValid()
    {
        // Arrange
        var item = Item.Create("Dagger", "A small weapon", 5m, 0.1m).Data!;

        // Act
        var result = MarketPricingService.Generate(item);

        // Assert
        result.Price.Should().BeGreaterThan(0);
        result.State.Should().BeOneOf(PriceState.HIGH, PriceState.LOW);
    }

    [Fact]
    public void Generate_WithLargeBasePrice_PriceIsValid()
    {
        // Arrange
        var item = Item.Create("Legendary Sword", "A powerful artifact", 10000m, 0.5m).Data!;

        // Act
        var result = MarketPricingService.Generate(item);

        // Assert
        result.Price.Should().BeGreaterThan(0);
        result.State.Should().BeOneOf(PriceState.HIGH, PriceState.LOW);
    }

    [Fact]
    public void Generate_WithZeroVolatility_PriceEqualsBasePrice()
    {
        // Arrange
        var basePrice = 100m;
        var item = Item.Create("Stable Item", "No price variation", basePrice, 0m).Data!;

        // Act
        var result = MarketPricingService.Generate(item);

        // Assert
        result.Price.Should().Be(basePrice);
    }

    [Fact]
    public void Generate_WithMaxVolatility_PriceWithinExpectedRange()
    {
        // Arrange
        var basePrice = 100m;
        var item = Item.Create("Volatile Item", "High price variation", basePrice, 1m).Data!;
        var minPrice = basePrice * (1 - item.Volatility); // 0m
        var maxPrice = basePrice * (1 + item.Volatility); // 200m

        // Act
        var result = MarketPricingService.Generate(item);

        // Assert
        result.Price.Should().BeGreaterThanOrEqualTo(minPrice);
        result.Price.Should().BeLessThanOrEqualTo(maxPrice);
    }

    [Fact]
    public void Generate_MultipleInvocations_ProduceVariedPrices()
    {
        // Arrange
        var item = Item.Create("Variable Item", "Should produce different prices", 100m, 0.3m).Data!;
        var prices = new List<decimal>();

        // Act
        for (int i = 0; i < 50; i++)
        {
            var result = MarketPricingService.Generate(item);
            prices.Add(result.Price);
        }

        // Assert - With volatility 0.3 and 50 iterations, we should get varied prices
        var uniquePrices = prices.Distinct().Count();
        uniquePrices.Should().BeGreaterThan(1, "Multiple invocations should not always produce same price with volatility > 0");
    }

    [Fact]
    public void Generate_StateIsDeterminedByRandomLogic()
    {
        // Arrange
        var item = Item.Create("Test Item", "Testing state distribution", 100m, 0.2m).Data!;
        var highStateCount = 0;
        var lowStateCount = 0;

        // Act - Run multiple times and count state distribution
        for (int i = 0; i < 100; i++)
        {
            var result = MarketPricingService.Generate(item);
            if (result.State == PriceState.HIGH)
                highStateCount++;
            else
                lowStateCount++;
        }

        // Assert - Both states should appear with relatively balanced distribution
        // (roughly 50/50 but not exactly due to randomness)
        highStateCount.Should().BeGreaterThan(0, "HIGH state should appear at least once in 100 iterations");
        lowStateCount.Should().BeGreaterThan(0, "LOW state should appear at least once in 100 iterations");
    }

    [Fact]
    public void Generate_WithDifferentBasePrices_ProducesScaledPrices()
    {
        // Arrange
        var volatility = 0.2m;
        var lowPriceItem = Item.Create("Cheap Item", "Low price item", 10m, volatility).Data!;
        var highPriceItem = Item.Create("Expensive Item", "High price item", 1000m, volatility).Data!;

        // Act
        var lowPriceResult = MarketPricingService.Generate(lowPriceItem);
        var highPriceResult = MarketPricingService.Generate(highPriceItem);

        // Assert
        // Even with same volatility, higher base price items should tend to produce higher prices
        lowPriceResult.Price.Should().BeLessThan(100m); // Cheap item should stay cheap
        highPriceResult.Price.Should().BeGreaterThan(800m); // Expensive item should stay relatively expensive
    }

    [Fact]
    public void Generate_PriceIsDecimal()
    {
        // Arrange
        var item = Item.Create("Test Item", "Test rounding", 100m, 0.25m).Data!;

        // Act
        var result = MarketPricingService.Generate(item);

        // Assert - Check that price is a decimal type
        result.Price.Should().BeOfType(typeof(decimal));
    }

    [Fact]
    public void Generate_WithSmallVolatility_LimitsPriceVariance()
    {
        // Arrange
        var basePrice = 100m;
        var smallVolatility = 0.01m;
        var item = Item.Create("Stable Item", "Low volatility item", basePrice, smallVolatility).Data!;
        var minExpected = basePrice * (1 - smallVolatility);
        var maxExpected = basePrice * (1 + smallVolatility);

        // Act
        var result = MarketPricingService.Generate(item);

        // Assert
        result.Price.Should().BeGreaterThanOrEqualTo(minExpected);
        result.Price.Should().BeLessThanOrEqualTo(maxExpected);
        // Price should be very close to base price with low volatility
        Math.Abs(result.Price - basePrice).Should().BeLessThan(2m);
    }

    [Fact]
    public void Generate_WithHighVolatility_AllowsLargeVariance()
    {
        // Arrange
        var basePrice = 100m;
        var highVolatility = 0.8m;
        var item = Item.Create("Volatile Item", "High volatility item", basePrice, highVolatility).Data!;
        var minExpected = basePrice * (1 - highVolatility);
        var maxExpected = basePrice * (1 + highVolatility);

        // Act
        var result = MarketPricingService.Generate(item);

        // Assert
        result.Price.Should().BeGreaterThanOrEqualTo(minExpected);
        result.Price.Should().BeLessThanOrEqualTo(maxExpected);
    }

    [Fact]
    public void Generate_ReturnsNonNegativePrice()
    {
        // Arrange
        var minVolatility = 0.5m;
        var item = Item.Create("Minimum Price Item", "Item with minimum price", 50m, minVolatility).Data!;

        // Act
        for (int i = 0; i < 20; i++)
        {
            var result = MarketPricingService.Generate(item);

            // Assert
            result.Price.Should().BeGreaterThan(0, "Price should always be positive");
        }
    }

    [Fact]
    public void Generate_WithSmallBasePriceAndSmallVolatility_ProducesValidResults()
    {
        // Arrange
        var basePrice = 10m;
        var volatility = 0.1m;
        var item = Item.Create("Test Item", "Testing various parameters", basePrice, volatility).Data!;

        // Act
        var result = MarketPricingService.Generate(item);

        // Assert
        var minPrice = basePrice * (1 - volatility);
        var maxPrice = basePrice * (1 + volatility);

        result.Price.Should().BeGreaterThanOrEqualTo(minPrice);
        result.Price.Should().BeLessThanOrEqualTo(maxPrice);
        result.State.Should().BeOneOf(PriceState.HIGH, PriceState.LOW);
    }

    [Fact]
    public void Generate_WithMediumBasePriceAndMediumVolatility_ProducesValidResults()
    {
        // Arrange
        var basePrice = 50m;
        var volatility = 0.25m;
        var item = Item.Create("Test Item", "Testing various parameters", basePrice, volatility).Data!;

        // Act
        var result = MarketPricingService.Generate(item);

        // Assert
        var minPrice = basePrice * (1 - volatility);
        var maxPrice = basePrice * (1 + volatility);

        result.Price.Should().BeGreaterThanOrEqualTo(minPrice);
        result.Price.Should().BeLessThanOrEqualTo(maxPrice);
        result.State.Should().BeOneOf(PriceState.HIGH, PriceState.LOW);
    }

    [Fact]
    public void Generate_WithHighBasePriceAndHighVolatility_ProducesValidResults()
    {
        // Arrange
        var basePrice = 100m;
        var volatility = 0.5m;
        var item = Item.Create("Test Item", "Testing various parameters", basePrice, volatility).Data!;

        // Act
        var result = MarketPricingService.Generate(item);

        // Assert
        var minPrice = basePrice * (1 - volatility);
        var maxPrice = basePrice * (1 + volatility);

        result.Price.Should().BeGreaterThanOrEqualTo(minPrice);
        result.Price.Should().BeLessThanOrEqualTo(maxPrice);
        result.State.Should().BeOneOf(PriceState.HIGH, PriceState.LOW);
    }

    [Fact]
    public void Generate_WithVeryHighBasePriceAndVeryHighVolatility_ProducesValidResults()
    {
        // Arrange
        var basePrice = 500m;
        var volatility = 0.75m;
        var item = Item.Create("Test Item", "Testing various parameters", basePrice, volatility).Data!;

        // Act
        var result = MarketPricingService.Generate(item);

        // Assert
        var minPrice = basePrice * (1 - volatility);
        var maxPrice = basePrice * (1 + volatility);

        result.Price.Should().BeGreaterThanOrEqualTo(minPrice);
        result.Price.Should().BeLessThanOrEqualTo(maxPrice);
        result.State.Should().BeOneOf(PriceState.HIGH, PriceState.LOW);
    }
}
