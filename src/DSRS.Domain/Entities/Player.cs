using DSRS.SharedKernel.Abstractions;
using DSRS.SharedKernel.Primitives;

namespace DSRS.Domain.Entities;

public class Player : EntityBase<Guid>
{
    public string Name { get; private set; } = string.Empty;
    public decimal Balance { get; private set; }

    public static Result<Player> Create(string name, decimal balance)
    {
        if(string.IsNullOrWhiteSpace(name))
            return Result<Player>.Failure(
                new Error("Player.Name.Empty", "Player name cannot be empty"));


        // maybe add domain event here

        return Result<Player>.Success(new Player { Name = name, Balance = balance });
    }

}
