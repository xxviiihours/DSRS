using DSRS.Application.Contracts;
using DSRS.Domain.Aggregates.Distributions;
using DSRS.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSRS.Infrastructure.Persistence.Repositories;

public class DistributionHistoryRepository(AppDbContext context) : IDistributionHistoryRepository
{
    private readonly AppDbContext _context = context;

    public async Task CreateAsync(DistributionRecord record)
    {
        await _context.DistributionRecords.AddAsync(record);

        await Task.CompletedTask;
    }
}
