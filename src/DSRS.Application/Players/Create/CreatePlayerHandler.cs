using DSRS.Application.Interfaces;
using DSRS.Domain.Players;
using DSRS.SharedKernel.Primitives;
using Mediator;
using System.Data.Common;

namespace DSRS.Application.Players.Create;

public class CreatePlayerHandler(IPlayerRepository playerRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreatePlayerCommand, Result<Player>>
{
    private readonly IPlayerRepository _playerRepository = playerRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async ValueTask<Result<Player>> Handle(CreatePlayerCommand command, CancellationToken cancellationToken)
    {
        try
        {
            if(await _playerRepository.NameExistsAsync(command.Name))
            {
                return Result<Player>.Failure(new Error("Player.Name.Exists",
                    $"A player with the name '{command.Name}' already exists."));
            }

            var player = Player.Create(command.Name, command.Balance);

            if (!player.IsSuccess)
                return Result<Player>.Failure(player.Error!);

            await _playerRepository.CreateAsync(player.Data!);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Result<Player>.Success(player.Data!);
        }
        catch (DbException ex)
        {
            return Result<Player>.Failure(new Error("Database.Error",
                $"{ex.Message}"));
        }

    }
}
