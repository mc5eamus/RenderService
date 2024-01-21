using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RenderDomain.Config;
using RenderDomain.Contract;
using System.Text.Json;


namespace RenderDomain.Middleware
{
    /// <summary>
    /// Implementation of a queue sender using Azure Service Bus.
    /// </summary>
    public class QueueSender : IQueueSender
    {
        private readonly ServiceBusClient client;
        private readonly ServiceBusSender sender;
        private readonly OutboundServiceBusClientConfig config;
        private readonly ILogger logger;

        public QueueSender(IOptions<OutboundServiceBusClientConfig> config, ILogger<QueueSender> logger)
        {
            this.config = config.Value;

            client = new ServiceBusClient(this.config.ConnectionString);

            sender = client.CreateSender(this.config.QueueName);

            this.logger = logger;
        }

        public async Task EnqueuePayload<T>(T payload) where T : class
        {
            var sender = client.CreateSender(config.QueueName);

            var msg = new ServiceBusMessage(JsonSerializer.Serialize(payload));

            await sender.SendMessageAsync(msg);

            this.logger.LogInformation("Enqueued message: {0}", msg.Body.ToString());
        }
    }
}
