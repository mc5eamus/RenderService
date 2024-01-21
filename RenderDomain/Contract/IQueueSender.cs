namespace RenderDomain.Contract
{
    /// <summary>
    /// Describes sending payloads to a queue
    /// </summary>
    public interface IQueueSender
    {
        Task EnqueuePayload<T>(T payload) where T : class;
    }
}