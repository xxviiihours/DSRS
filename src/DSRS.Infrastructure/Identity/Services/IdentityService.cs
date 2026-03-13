using DSRS.Application.Contracts;
using DSRS.Domain.Aggregates.Players;
using DSRS.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace DSRS.Infrastructure.Identity.Services;

internal class IdentityService(UserManager<ApplicationUser> userManager, 
    SignInManager<ApplicationUser> signInManager, IPlayerRepository playerRepository) : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly IPlayerRepository _playerRepository = playerRepository;

    public Task<string> Authenticate(string username, string password)
    {
        throw new NotImplementedException();
    }

    public async Task RegisterAccount(Player player, string email, string password)
    {
        var user = new ApplicationUser { UserName = player.Name, Email = email, PlayerId = player.Id };

        var result = await _userManager.CreateAsync(user, password);

        if(!result.Succeeded)
            throw new Exception(result.Errors.FirstOrDefault()?.Description);

        await Task.CompletedTask;

    }
}
