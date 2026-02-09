using DSRS.SharedKernel.Abstractions;
using DSRS.SharedKernel.Primitives;

namespace DSRS.Domain.Entities;

public class Item : EntityBase<Guid>
{
    public string Name { get; private set; } = string.Empty;
    public decimal BasePrice { get; private set; }
    public decimal Volatility { get; private set; }

    public static Result<Item> Create(string name, decimal basePrice, decimal volatility)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<Item>.Failure(
                new Error("Item.Name.Empty", "Item name cannot be empty"));

        if (basePrice <= 0)
            return Result<Item>.Failure(
                new Error("Item.BasePrice.Invalid", "Base price must be greater than zero"));


        if (volatility < 0 || volatility > 1)
            return Result<Item>.Failure(
                new Error("Item.Volatility.Invalid", "Invalid volatility value"));
                
        // domain event could be raised here, e.g., ItemCreated

        return Result<Item>.Success(
            new Item
            {
                Name = name,
                BasePrice = basePrice,
                Volatility = volatility
            });
    }
}
