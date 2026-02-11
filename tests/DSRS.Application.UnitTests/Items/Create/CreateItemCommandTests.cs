using DSRS.Application.Items.Create;
using FluentAssertions;
using Xunit;

namespace DSRS.Application.UnitTests.Items.Create;

public class CreateItemCommandTests
{
    [Fact]
    public void Construct_AssignsProperties()
    {
        var cmd = new CreateItemCommand("Sword", "a sturdy weapon capable of mass destruction", 5m, 0.1m);
        cmd.Name.Should().Be("Sword");
        cmd.Description.Should().Be("a sturdy weapon capable of mass destruction");
        cmd.BasePrice.Should().Be(5m);
        cmd.Volatility.Should().Be(0.1m);
    }

    [Fact]
    public void Records_WithSameValues_AreEqual()
    {
        var a = new CreateItemCommand("Sword", "sturdy weapon capable of mass destruction", 5m, 0.1m);
        var b = new CreateItemCommand("Sword", "sturdy weapon capable of mass destruction", 5m, 0.1m);
        a.Should().Be(b);
        (a == b).Should().BeTrue();
    }

    [Fact]
    public void Deconstruct_Works()
    {
        var cmd = new CreateItemCommand("Shield", "A sturdy secondary weapon capable of blocking heavy attacks", 10m, 0.25m);
        var (name, description, basePrice, volatility) = cmd;
        name.Should().Be("Shield");
        description.Should().Be("A sturdy secondary weapon capable of blocking heavy attacks");
        basePrice.Should().Be(10m);
        volatility.Should().Be(0.25m);
    }

    [Fact]
    public void DifferentValues_AreNotEqual()
    {
        var a = new CreateItemCommand("Sword", "sturdy weapon capable of mass destruction", 5m, 0.1m);
        var b = new CreateItemCommand("Axe", "A multi-purpose arms capable of chopping woods and head hehe", 5m, 0.1m);
        a.Should().NotBe(b);
    }
}
