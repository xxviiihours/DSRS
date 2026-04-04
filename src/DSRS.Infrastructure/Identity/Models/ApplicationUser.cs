using DSRS.Domain.Aggregates.Players;
using DSRS.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSRS.Infrastructure.Identity.Models;

public class ApplicationUser : IdentityUser<Guid>
{
    public PlayerId PlayerId { get; set; }
    public Player Player { get; set; } = null!;
}
