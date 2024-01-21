namespace RenderDomain.Config
{
    /// <summary>
    /// We're using Cosmos DB for our render task repository,
    /// feel free to switch implementation to your DB
    /// </summary>
    public class CosmosRenderTaskRepoConfig
    {
        public required string Endpoint { get; set; }
        public required string Key { get; set; }
        public required string DatabaseId { get; set; }
        public required string ContainerId { get; set; }
    }
}
