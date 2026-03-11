using DSRS.Domain.Aggregates.Players;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSRS.Infrastructure.Identity.Models;

public class ApplicationUser : IdentityUser<Guid>
{
    public Guid PlayerId { get; set; }
    public Player Player { get; set; } = null!;
}
