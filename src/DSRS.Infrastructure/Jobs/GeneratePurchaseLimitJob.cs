using DSRS.Application.Contracts;
using DSRS.Application.Options;
using DSRS.Infrastructure.Persistence;
using DSRS.Infrastructure.Persistence.Services;
using DSRS.SharedKernel.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSRS.Infrastructure.Jobs;

public class GeneratePurchaseLimitJob(
    IServiceScopeFactory scopeFactory,
    IDateTime dateTimeService,
    ILogger<GeneratePurchaseLimitJob> logger,
    IOptions<BackgroundJobOption> backgroundJobOption) : IJob
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly IDateTime _dateTimeService = dateTimeService;

    private readonly ILogger<GeneratePurchaseLimitJob> _logger = logger;
    private readonly BackgroundJobOption _backgroundJobOption = backgroundJobOption.Value;
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            if (!_backgroundJobOption.Enabled)
                return;

            var start = _dateTimeService.UtcNow;
            _logger.LogInformation("PurchaseLimitJob started at {Time}", start);

            await using var scope = _scopeFactory.CreateAsyncScope();
            var marketService = scope.ServiceProvider.GetRequiredService<IMarketService>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var today = _dateTimeService.DateTodayUtc;

            var generatedCount = await marketService.GeneratePurchaseLimitAsync(today);

            await unitOfWork.CommitAsync();

            _logger.LogInformation(
               "PurchaseLimitJob completed. Generated {Count} players in {Duration}ms",
               generatedCount,
               (DateTime.UtcNow - start).TotalMilliseconds);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PurchaseLimitJob failed.");
            throw;
        }
    }
}
