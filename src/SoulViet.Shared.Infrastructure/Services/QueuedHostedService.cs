using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SoulViet.Shared.Application.Interfaces;

namespace SoulViet.Shared.Infrastructure.Services
{
    public class QueuedHostedService : BackgroundService
    {
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly ILogger<QueuedHostedService> _logger;
        public QueuedHostedService(IBackgroundTaskQueue taskQueue, ILogger<QueuedHostedService> logger)
        {
            _taskQueue = taskQueue;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Background Hosted Service for Syncing DB is running.");

            while(!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var workItem = await _taskQueue.DequeueAsync(stoppingToken);
                    await workItem(stoppingToken);
                } 
                catch (OperationCanceledException)
                {
                    //
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred executing background DB Sync task.");
                }
            }
        }
    }
}