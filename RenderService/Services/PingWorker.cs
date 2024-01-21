using RenderDomain.Contract;

namespace RenderService.Services
{

    /// <summary>
    /// Pats the queue consumer on the shoulder every 5 seconds
    /// </summary>
    public class PingWorker : BackgroundService
    {
        private readonly IQueueConsumer qs;
        private readonly ILogger logger;

        public PingWorker(IQueueConsumer qs, ILogger<PingWorker> logger)
        {
            this.qs = qs;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await qs.Ping(stoppingToken);
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
