using System;

namespace DSRS.SharedKernel.Interfaces;

public interface IDateTime
{

  public DateTime Now { get; }
  public DateTime UtcNow { get; }
  public DateOnly DateToday { get; }
}
