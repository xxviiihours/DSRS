using DSRS.Application.Contracts;
using DSRS.Application.Exceptions;
using DSRS.Application.Features.Players;
using DSRS.Domain.Aggregates.Players;
using DSRS.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace DSRS.Infrastructure.Identity.Services;

public class IdentityService(UserManager<ApplicationUser> userManager,
    IHttpContextAccessor httpContextAccessor, IPlayerQuery playerQuery) : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IPlayerQuery playerQuery = playerQuery;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task<PlayerDto> Authenticate(string username, string password)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null)
            throw new UnauthorizedException("Invalid username or password.");

        var passwordValid = await _userManager.CheckPasswordAsync(user, password);
        if (!passwordValid)
            throw new UnauthorizedException("Invalid username or password.");

        var player = await playerQuery.GetPlayerByIdAsync(user.PlayerId);

        var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, user.PlayerId.ToString()),
        new(ClaimTypes.Name, player.Name),
        new("is_guest", player.IsGuest.ToString())
    };

        var identity = new ClaimsIdentity(
            claims,
            IdentityConstants.ApplicationScheme
        );

        var principal = new ClaimsPrincipal(identity);

        await _httpContextAccessor.HttpContext!.SignInAsync(
            IdentityConstants.ApplicationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30)
            });

        return player;
    }

    public async Task<Guid> FindByIdAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            throw new UnauthorizedException("Invalid username or password.");

        return user.PlayerId;
    }

    public async Task RegisterAccount(Player player, string email, string password)
    {
        var user = new ApplicationUser { UserName = player.Name, Email = email, PlayerId = player.Id };

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
            throw new Exception(result.Errors.FirstOrDefault()?.Description);

        await Task.CompletedTask;

    }
}
