using DSRS.Application.Contracts;
using DSRS.Domain.Inventories;
using DSRS.Infrastructure.Persistence;
using Microsoft.Extensions.Logging;
using System;

namespace DSRS.Infrastructure.Repositories;

public class DistributionRepository(AppDbContext context,
    ILogger<DistributionRepository> logger) : IDistributionRepository
{
    private readonly AppDbContext _context = context;
    private readonly ILogger<DistributionRepository> _logger = logger;

    public async Task CreateAsync(Inventory inventory)
    {
        await _context.AddAsync(inventory);

        await Task.CompletedTask;
    }

    public async Task CreateBatchAsync(List<Inventory> inventories)
    {
        
        await _context.AddRangeAsync(inventories);
        _logger.LogInformation("Task Add Batch applied.");

        
        await Task.CompletedTask;
        _logger.LogInformation($"Task: {nameof(Inventory)} Add Batch completed.");
    }
}
