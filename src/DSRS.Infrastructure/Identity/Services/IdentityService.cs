using DSRS.Application.Contracts;
using DSRS.Domain.Aggregates.Players;
using DSRS.Infrastructure.Identity.Models;
using DSRS.SharedKernel.Primitives;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

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

    public async Task RegisterAccount(Player player, string password)
    {
        var user = new ApplicationUser { UserName = player.Name, PlayerId = player.Id };

        var result = await _userManager.CreateAsync(user, password);

        if(!result.Succeeded)
            throw new Exception(result.Errors.FirstOrDefault()?.Description);

        await Task.CompletedTask;

    }
}
