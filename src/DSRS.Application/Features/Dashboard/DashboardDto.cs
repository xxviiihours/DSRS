using DSRS.Domain.Distributions;
using DSRS.SharedKernel.Enums;
using System;

namespace DSRS.Application.Features.Dashboard;

public class DashboardDto
{
    public decimal BasePrice { get; set; }
    public decimal PreviousPrice { get; set; }
    public decimal Percentage { get; set; }
    public PriceState State { get; set; }
    public DateOnly Date { get; set; }
}
