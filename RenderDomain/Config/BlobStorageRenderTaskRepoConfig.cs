namespace RenderDomain.Config
{
    /// <summary>
    /// Configuration for the Render Task File Repository
    /// </summary>
    public class BlobStorageRenderTaskRepoConfig
    {
        public required string ConnectionString { get; set; }
        public required string InboundContainerName { get; set; }
        public required string OutboundContainerName { get; set; }
    }
}
