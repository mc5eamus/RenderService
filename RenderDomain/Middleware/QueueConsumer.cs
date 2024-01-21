using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RenderDomain.Config;
using RenderDomain.Contract;

namespace RenderService.Middleware
{
    /// <summary>
    /// Implementation of a queue consumer using Azure Service Bus.
    /// It keeps tracks of the last message it received and can renew the lock on that message.
    /// </summary>
    public class QueueConsumer : IDisposable, IQueueConsumer
    {
        private readonly ServiceBusClient client;
        private readonly ServiceBusReceiver receiver;
        private readonly InboundServiceBusClientConfig config;
        private readonly TimeSpan timeSpan;
        private readonly ILogger logger;

        private object msgLock = new object();
        private ServiceBusReceivedMessage? msg;

        public QueueConsumer(IOptions<InboundServiceBusClientConfig> config, ILogger<QueueConsumer> logger)
        {
            this.config = config.Value;
            this.client = new ServiceBusClient(this.config.ConnectionString);
            this.receiver = client.CreateReceiver(this.config.QueueName);
            this.timeSpan = TimeSpan.FromSeconds(this.config.Timeout);
            this.logger = logger;
        }

        /// <summary>
        /// Gets the next message from the queue, or waits config.Timeout seconds for one and returns null if none is found.
        /// </summary>
        /// <typeparam name="T">type to convert the message payload to.</typeparam>
        /// <param name="token">cancellation token</param>
        /// <returns></returns>
        public async Task<T?> GetNext<T>(CancellationToken token) where T : class
        {
            var msg = await receiver.ReceiveMessageAsync(this.timeSpan, token);
            if (msg == null)
            {
                return null;
            }

            try
            {
                var res = msg.Body.ToObjectFromJson<T>();
                lock (msgLock)
                {
                    this.msg = msg;
                }
                return res;
            }
            catch (Exception ex)
            {
                await receiver.CompleteMessageAsync(msg, token);
                logger.LogError(ex, "Failed to deserialize message: {0}", msg.Body.ToString());
                return null;
            }
        }

        /// <summary>
        /// Renews the lock on the last message received.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task Ping(CancellationToken token)
        {
            if (msg != null)
            {
                if ((msg.LockedUntil - DateTime.UtcNow) < TimeSpan.FromSeconds(30))
                {
                    await receiver.RenewMessageLockAsync(this.msg, token);
                }
            }
        }

        /// <summary>
        /// Marks the message as completed.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task Complete(CancellationToken token)
        {
            await receiver.CompleteMessageAsync(msg, token);

            lock (msgLock)
            {
                this.msg = null;
            }

        }

        public void Dispose()
        {
            if (!receiver.IsClosed)
            {
                logger.LogInformation("Closing receiver");
                receiver.CloseAsync();
            }
        }

        /// <summary>
        /// Is my consumer currently busy working on something, and if yes, what is it? 
        /// </summary>
        /// <returns></returns>
        public string GetStatus()
        {
            lock (msgLock)
            {
                if(msg == null)
                {
                    return "no active job";
                }

                return $"active job: {msg.Body.ToString()}";
            }
        }
    }
}
