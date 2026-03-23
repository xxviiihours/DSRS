using DSRS.Domain.Aggregates.Distributions;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSRS.Application.Contracts;

public interface IDistributionHistoryRepository
{
    Task CreateAsync(DistributionRecord record);
}
