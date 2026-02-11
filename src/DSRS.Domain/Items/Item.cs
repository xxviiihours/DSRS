using DSRS.Domain.Pricing;
using DSRS.SharedKernel.Abstractions;
using DSRS.SharedKernel.Enums;
using DSRS.SharedKernel.Primitives;

namespace DSRS.Domain.Items;

public sealed class Item : EntityBase<Guid>
{
    public string Name { get; } = string.Empty;
    public string Description { get; }
    public decimal BasePrice { get; }
    public decimal Volatility { get; }

    private Item(string name, string description, decimal basePrice, decimal volatility)
    {
        Name = name;
        Description = description;
        BasePrice = basePrice;
        Volatility = volatility;
    }
    public static Result<Item> Create(string name, string description, decimal basePrice, decimal volatility)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<Item>.Failure(
                new Error("Item.Name.Empty", "Item name cannot be empty"));

        if (string.IsNullOrWhiteSpace(description))
            return Result<Item>.Failure(
               new Error("Item.Description.Empty", "Item description cannot be empty"));

        if (basePrice <= 0)
            return Result<Item>.Failure(
                new Error("Item.BasePrice.Invalid", "Base price must be greater than zero"));


        if (volatility < 0 || volatility > 1)
            return Result<Item>.Failure(
                new Error("Item.Volatility.Invalid", "Invalid volatility value"));

        // domain event could be raised here, e.g., ItemCreated

        return Result<Item>.Success(new Item(name, description, basePrice, volatility));
    }
}
