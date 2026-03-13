using DSRS.Application.Contracts;
using DSRS.Domain.Aggregates.Players;
using DSRS.SharedKernel.Primitives;
using Mediator;
using System.Numerics;
using System.Xml.Linq;

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
            Player player;

            var nameExists = await _playerRepository.NameExistsAsync(command.Username);

            if (nameExists)
                return Result<Player>.Failure(
                    new Error("Player.Username.Exists", "Username already exists."));

            if (command.Id != Guid.Empty)
            {
                player = await _playerRepository.FindGuestById(command.Id);

                if (player == null)
                    return Result<Player>.Failure(
                        new Error("Player.Id.NotFound", "Player not found."));

                player.UpdateName(command.Username);
            }
            else
            {
                player = Player.Create(command.Username).Data!;
                await _playerRepository.CreateAsync(player);

            }

            player.Register();

            await _identityService.RegisterAccount(player, command.Email, command.Password);
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
