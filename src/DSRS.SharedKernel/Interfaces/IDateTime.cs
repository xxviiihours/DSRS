using System;

namespace DSRS.SharedKernel.Interfaces;

public interface IDateTime
{

  public DateTime Now { get; }
  public DateOnly DateToday { get; }
}
