using DSRS.Domain.Aggregates.Distributions;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSRS.Application.Features.Dashboard;

public class TradeActivityDto
{
    public string ItemName { get; set; } = string.Empty;
    public decimal PriceTotal { get; set; }
    public int TotalTrades { get; set; }
    public DistributionType Type { get; set; }
    public DateTime TransactionDate { get; set; }
}
