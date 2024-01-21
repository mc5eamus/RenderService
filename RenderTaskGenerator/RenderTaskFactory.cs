using Microsoft.Extensions.Logging;
using RenderDomain.Contract;
using RenderDomain.Model;
using System.Text;

namespace RenderTaskGenerator
{
    public class RenderTaskFactory
    {
        private readonly IRenderTaskOrchestrator orchestrator;
        private readonly IQueueSender queueSender;
        private readonly ILogger logger;

        public RenderTaskFactory(
            IRenderTaskOrchestrator orchestrator,
            IQueueSender queueSender,
            ILogger<RenderTaskFactory> logger)
        {
            this.orchestrator = orchestrator;
            this.queueSender = queueSender;
            this.logger = logger;
        }

        public async Task Run(int num)
        {
            for(int i = 0; i < num; i++)
            {
                await CreateRenderTask(Guid.NewGuid().ToString());
            }
        }

        private async Task CreateRenderTask(string id)
        {
            var payload = new System.IO.MemoryStream(Encoding.UTF8.GetBytes($"<SomeXmlData id=\"{id}\">payload</SomeXmlData>"));

            var task = await orchestrator.CreateRenderTaskAsync(new RenderTask
            {
                Id = id,
                Status = RenderTaskStatus.Queued
            }, payload);

            await queueSender.EnqueuePayload(task);
        }

    }
}
