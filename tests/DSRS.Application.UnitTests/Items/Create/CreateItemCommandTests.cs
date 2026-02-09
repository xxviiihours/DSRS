using DSRS.Application.Items.Create;
using FluentAssertions;
using Xunit;

namespace DSRS.Application.UnitTests.Items.Create;

public class CreateItemCommandTests
{
    [Fact]
    public void Construct_AssignsProperties()
    {
        var cmd = new CreateItemCommand("Sword", 5m, 0.1m);
        cmd.Name.Should().Be("Sword");
        cmd.BasePrice.Should().Be(5m);
        cmd.Volatility.Should().Be(0.1m);
    }

    [Fact]
    public void Records_WithSameValues_AreEqual()
    {
        var a = new CreateItemCommand("Sword", 5m, 0.1m);
        var b = new CreateItemCommand("Sword", 5m, 0.1m);
        a.Should().Be(b);
        (a == b).Should().BeTrue();
    }

    [Fact]
    public void Deconstruct_Works()
    {
        var cmd = new CreateItemCommand("Shield", 10m, 0.25m);
        var (name, basePrice, volatility) = cmd;
        name.Should().Be("Shield");
        basePrice.Should().Be(10m);
        volatility.Should().Be(0.25m);
    }

    [Fact]
    public void DifferentValues_AreNotEqual()
    {
        var a = new CreateItemCommand("Sword", 5m, 0.1m);
        var b = new CreateItemCommand("Axe", 5m, 0.1m);
        a.Should().NotBe(b);
    }
}
