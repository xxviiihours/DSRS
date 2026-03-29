using DSRS.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSRS.Infrastructure.Extensions;

public static class VogenPropertyBuilderExtensions
{

    public static PropertyBuilder<Money> HasMoneyConversion(
        this PropertyBuilder<Money> builder, int precision = 18, int scale = 2)
    {
        return builder
            .HasConversion(x => x.Value, x => Money.From(x))
            .HasPrecision(precision, scale);
    }
}
