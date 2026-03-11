using DSRS.Application.Contracts;
using DSRS.Domain.Aggregates.Players;
using DSRS.SharedKernel.Primitives;
using Mediator;

namespace DSRS.Application.Features.Accounts.Register;

public class RegisterAccountHandler(IPlayerRepository playerRepository,
    IIdentityService identityService, IUnitOfWork unitOfWork) : ICommandHandler<RegisterAccountCommand, Result<Player>>
{
    private readonly IPlayerRepository _playerRepository = playerRepository;
    private readonly IIdentityService _identityService = identityService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async ValueTask<Result<Player>> Handle(RegisterAccountCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var player = await _playerRepository.GetById(command.Id);

            if (player == null)
                return Result<Player>.Failure(
                    new Error("Player.Id.NotFound", "Player not found."));

            await _identityService.RegisterAccount(player, command.Password);

            player.Register();

            await _unitOfWork.CommitAsync(cancellationToken);

            return Result<Player>.Success(player);
        }
        catch (Exception ex)
        {
            return Result<Player>.Failure(
                new Error("Player.RegistrationFailed", $"Player registration failed: {ex.Message}"));
        }

    }
}
