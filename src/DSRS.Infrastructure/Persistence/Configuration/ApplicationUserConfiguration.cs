using DSRS.Infrastructure.Extensions;
using DSRS.Infrastructure.Identity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSRS.Infrastructure.Persistence.Configuration;

internal class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(x => x.PlayerId)
            .HasPlayerIdConversion()
            .ValueGeneratedNever()
            .IsRequired();

        builder.HasOne(x => x.Player)
            .WithOne()
            .HasForeignKey<ApplicationUser>(x => x.PlayerId);
    }
}
