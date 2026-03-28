using System;
using System.Collections.Generic;
using System.Text;

namespace DSRS.Application.Features.Dashboard;

public class BalancePerformanceDto
{
    public decimal Balance { get; set; }
    public decimal Profit { get; set; }
    public DateTime Day { get; set; }
}
