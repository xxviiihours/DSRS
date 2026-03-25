using DSRS.Application.Contracts;
using DSRS.Domain.Aggregates.Players;
using DSRS.Domain.Services;
using DSRS.SharedKernel.Interfaces;
using DSRS.SharedKernel.Primitives;
using Mediator;
using System.Data.Common;

namespace DSRS.Application.Features.Players.CreatePlayer;

public class CreatePlayerHandler(IPlayerRepository playerRepository,
    IUnitOfWork unitOfWork, IDateTime datetimeService) : ICommandHandler<CreatePlayerCommand, Result<Player>>
{
    private readonly IPlayerRepository _playerRepository = playerRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IDateTime _datetimeService = datetimeService;

    public async ValueTask<Result<Player>> Handle(CreatePlayerCommand command, CancellationToken cancellationToken)
    {
        try
        {
            if (await _playerRepository.NameExistsAsync(command.Name))
            {
                return Result<Player>.Failure(new Error("Player.Name.Exists",
                    $"A player with the name '{command.Name}' already exists."));
            }

            var player = Player.Create(command.Name);

            if (!player.IsSuccess)
                return Result<Player>.Failure(player.Error!);

            var today = DateOnly.FromDateTime(_datetimeService.Now);
            PlayerPurchaseService.GenerateDailyPurchaseLimit(player.Data!, today);

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
