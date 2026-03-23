using DSRS.Application.Contracts;
using DSRS.SharedKernel.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;

namespace DSRS.Infrastructure.Jobs;

[DisallowConcurrentExecution]
public class GeneratePriceJob(
    IServiceScopeFactory scopeFactory,
    IDateTime dateTimeService,
    IConfiguration configuration,
    ILogger<GeneratePriceJob> logger) : IJob
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly IDateTime _dateTimeService = dateTimeService;
    private readonly IConfiguration _configuration = configuration;

    private readonly ILogger<GeneratePriceJob> _logger = logger;

    public async Task Execute(IJobExecutionContext context)
    {
        var isEnabled = _configuration.GetValue<bool>("BackgroundJobs:Enabled");

        if (!isEnabled)
            return;

        var start = _dateTimeService.UtcNow;
        _logger.LogInformation("DailyPriceJob started at {Time}", start);
        try
        {
            await using var scope = _scopeFactory.CreateAsyncScope();

            var marketService = scope.ServiceProvider.GetRequiredService<IMarketService>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            int generatedCount = await marketService.GenerateDailyPricesAsync(_dateTimeService.DateTodayUtc);
            await unitOfWork.CommitAsync();

            _logger.LogInformation(
                "DailyPriceJob completed. Generated {Count} prices in {Duration}ms",
                generatedCount,
                (DateTime.UtcNow - start).TotalMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DailyPriceJob failed");
            throw;
        }
    }
}
