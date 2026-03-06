using DSRS.SharedKernel.Interfaces;
using System;

namespace DSRS.Infrastructure.Persistence.Services;

public class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;

    public DateOnly DateToday => DateOnly.FromDateTime(Now);

    public DateTime UtcNow => DateTime.UtcNow;
}
