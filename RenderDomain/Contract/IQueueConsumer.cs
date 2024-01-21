namespace RenderDomain.Contract
{

    /// <summary>
    /// A contract for anyone consuming items from a queue
    /// </summary>
    public interface IQueueConsumer
    {
        Task Complete(CancellationToken token);
        void Dispose();
        Task<T?> GetNext<T>(CancellationToken token) where T : class;
        Task Ping(CancellationToken token);

        string GetStatus();
    }
}