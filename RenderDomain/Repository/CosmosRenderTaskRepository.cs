using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RenderDomain.Config;
using RenderDomain.Contract;
using RenderDomain.Model;

namespace RenderDomain.Repository
{
    /// <summary>
    /// Reference implementation of a task repository using Azure Cosmos DB
    /// </summary>
    public class CosmosRenderTaskRepository : IRenderTaskRepository
    {
        private readonly CosmosClient cosmosClient;
        private readonly CosmosRenderTaskRepoConfig config;
        private readonly ILogger logger;
        
        public CosmosRenderTaskRepository(
            IOptions<CosmosRenderTaskRepoConfig> options, 
            ILogger<CosmosRenderTaskRepository> logger)
        {
            config = options.Value;
            cosmosClient = new CosmosClient(
                options.Value.Endpoint,
                new Azure.AzureKeyCredential(options.Value.Key),
                new CosmosClientOptions()
                {
                    SerializerOptions = new CosmosSerializationOptions()
                    {
                        PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase,
                    }
                });
            this.logger = logger;
        }

        public async Task<RenderTask> CreateRenderTaskAsync(RenderTask renderTask)
        {
            var itemResponse = await cosmosClient.GetContainer(config.DatabaseId, config.ContainerId).CreateItemAsync(renderTask);
            return itemResponse.Resource;
        }

        public async Task DeleteRenderTaskAsync(string id)
        {
            await cosmosClient.GetContainer(config.DatabaseId, config.ContainerId).DeleteItemAsync<RenderTask>(id, new PartitionKey(id));
        }

        public async Task<RenderTask> GetRenderTaskAsync(string id)
        {
            try { 
                var itemResponse =  await cosmosClient.GetContainer(config.DatabaseId, config.ContainerId).ReadItemAsync<RenderTask>(id, new PartitionKey(id));
                return itemResponse.Resource;
            } catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }
        public async Task<RenderTask> UpdateRenderTaskAsync(RenderTask renderTask)
        {
            var itemResponse = await cosmosClient.GetContainer(config.DatabaseId, config.ContainerId).UpsertItemAsync(renderTask);
            return itemResponse.Resource;
        }
    }
}
