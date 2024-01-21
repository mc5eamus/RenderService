
namespace RenderDomain.Config
{
    /// <summary>
    /// Service Bus Client Configuration
    /// </summary>
    public class ServiceBusClientConfig
    {
        public required string ConnectionString { get; set; }
        public required string QueueName { get; set; }

    }

    /// <summary>
    /// Config override for queue consumer 
    /// </summary>
    public class InboundServiceBusClientConfig : ServiceBusClientConfig
    {
        /// <summary>
        /// How long to wait for messages before timing out
        /// </summary>
        public int Timeout { get; set; }
    }

    /// <summary>
    /// Config override for queue producer
    /// </summary>
    public class OutboundServiceBusClientConfig : ServiceBusClientConfig
    {
    }


}
