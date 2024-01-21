using RenderDomain.Contract;
using RenderDomain.Model;
using System.IO.Compression;

namespace RenderService.Services
{
    /// <summary>
    /// The actual worker service that processes render tasks
    /// </summary>
    public class RenderWorker : BackgroundService
    {
        private readonly IRenderTaskOrchestrator orchestrator;
        private readonly IQueueConsumer mwConsumer;
        private readonly IQueueSender mwSender;
        private readonly ILogger<RenderWorker> logger;

        public RenderWorker(
            IRenderTaskOrchestrator orchestrator, 
            IQueueConsumer mwConsumer, 
            IQueueSender mwSender,
            ILogger<RenderWorker> logger)
        {
            this.orchestrator = orchestrator;
            this.mwConsumer = mwConsumer;
            this.mwSender = mwSender;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var task = await mwConsumer.GetNext<RenderTask>(stoppingToken);
                if (task != null)
                {
                    logger.LogInformation("Got task {0}", task.Id);
                    await orchestrator.UpdateTaskStatus(task.Id, RenderTaskStatus.Processing, Environment.MachineName);
                    
                    
                    // entry point for the actual rendering
                    await Task.Delay(80000, stoppingToken);

                    // simulate rendering result by creating a zip file
                    // containing a single file called result.xml
                    using (var memoryStream = new MemoryStream())
                    {
                        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                        {
                            var resultFile = archive.CreateEntry("result.xml");

                            using (var entryStream = resultFile.Open())
                            using (var streamWriter = new StreamWriter(entryStream))
                            {
                                streamWriter.Write($"<RenderResult id=\"{task.Id}\">result</RenderResult>");
                            }
                        }

                        memoryStream.Seek(0, SeekOrigin.Begin);
                        await orchestrator.CompleteTask(task.Id, memoryStream, Environment.MachineName);
                    }

                    // now we are done with the task, update its status
                    // and send the message to the outbout queue
                    // informing the outer world about our progess
                    await orchestrator.UpdateTaskStatus(task.Id, RenderTaskStatus.Complete, Environment.MachineName);
                    await mwConsumer.Complete(stoppingToken);
                    await mwSender.EnqueuePayload(task);
                }
                else
                {
                    logger.LogInformation("No task");
                    await Task.Delay(10000, stoppingToken);
                }
            }
        }
    }
}
