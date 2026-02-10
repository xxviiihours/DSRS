using DSRS.SharedKernel.Abstractions;
using DSRS.SharedKernel.Primitives;

namespace DSRS.Domain.Entities;

public sealed class Player : EntityBase<Guid>
{
    public string Name { get; } = string.Empty;
    public decimal Balance { get; }
    private Player(string name, decimal balance)
    {
        Name = name;
        Balance = balance;
    }
    public static Result<Player> Create(string name, decimal balance)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<Player>.Failure(
                new Error("Player.Name.Empty", "Player name cannot be empty"));


        // maybe add domain event here

        return Result<Player>.Success(new Player(name, balance));
    }

}
