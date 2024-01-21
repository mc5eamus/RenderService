namespace RenderDomain.Contract
{
    /// <summary>
    /// Identifies actions against a file repository for storing render task related files
    /// </summary>
    public interface IRenderTaskFileRepository
    {
        Task<Stream> GetRenderTaskInput(string id);
        Task SaveRenderTaskOutput(string id, Stream stream);
        Task SaveRenderTaskInput(string id, Stream stream);
    }
}