namespace TechStoreWeb.Services;

/// <summary>
/// Background Worker that automatically retries failed price sync queue items every 30 seconds.
/// In a real distributed system, this replaces manual retry — the system self-heals
/// when the remote database comes back online.
/// </summary>
public sealed class PriceSyncBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PriceSyncBackgroundService> _logger;

    private static readonly TimeSpan RetryInterval = TimeSpan.FromSeconds(30);

    public PriceSyncBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<PriceSyncBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("[PriceSyncWorker] Started — will retry failed queue items every {Seconds}s.",
            RetryInterval.TotalSeconds);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(RetryInterval, stoppingToken);

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<TechStoreRepository>();
                await repository.ProcessQueueAsync();

                _logger.LogDebug("[PriceSyncWorker] Queue scan completed.");
            }
            catch (OperationCanceledException)
            {
                // Application shutting down — expected, don't log as error
                break;
            }
            catch (Exception ex)
            {
                // Log but don't crash — will retry on next cycle
                _logger.LogWarning(ex, "[PriceSyncWorker] Queue processing failed, will retry in {Seconds}s.",
                    RetryInterval.TotalSeconds);
            }
        }

        _logger.LogInformation("[PriceSyncWorker] Stopped.");
    }
}
