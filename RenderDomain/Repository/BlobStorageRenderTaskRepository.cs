using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RenderDomain.Config;
using RenderDomain.Contract;

namespace RenderDomain.Repository
{
    /// <summary>
    /// Imlements a file repository using Azure Blob Storage
    /// </summary>
    public class BlobStorageRenderTaskRepository : IRenderTaskFileRepository
    {
        private readonly BlobServiceClient blobServiceClient;
        private readonly BlobContainerClient inboundContainerClient;
        private readonly BlobContainerClient outboundContainerClient;
        private readonly ILogger logger;

        public BlobStorageRenderTaskRepository(
            IOptions<BlobStorageRenderTaskRepoConfig> options,
            ILogger<BlobStorageRenderTaskRepository> logger)
        {
            var config = options.Value;
            blobServiceClient = new BlobServiceClient(config.ConnectionString);
            inboundContainerClient = blobServiceClient.GetBlobContainerClient(config.InboundContainerName);
            outboundContainerClient = blobServiceClient.GetBlobContainerClient(config.OutboundContainerName);
            this.logger = logger;
        }

        /// <summary>
        /// Reads the content of a render task input file from the repository
        /// </summary>
        /// <param name="id">task id</param>
        /// <returns></returns>
        public async Task<Stream> GetRenderTaskInput(string id)
        {
            var blobClient = inboundContainerClient.GetBlobClient($"{id}.xml");
            var blobDownloadInfo = await blobClient.DownloadAsync();
            return blobDownloadInfo.Value.Content;
        }

        /// <summary>
        /// Stores the content of a render task output file in the repository, provided as stream
        /// </summary>
        /// <param name="id">task id</param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public async Task SaveRenderTaskOutput(string id, Stream stream)
        {
            var blobClient = outboundContainerClient.GetBlobClient($"{id}.zip");
            try
            {
                var result = await blobClient.UploadAsync(stream);
                logger.LogInformation("Uploaded {0} to {1}", id, result.GetRawResponse().Status);
            } catch (Exception ex)
            {
                logger.LogError(ex, "Failed to upload {0}", id);
            }
        }

        /// <summary>
        /// Stores the render task input file in the repository, provided as stream
        /// </summary>
        /// <param name="id"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public async Task SaveRenderTaskInput(string id, Stream stream)
        {
            var blobClient = inboundContainerClient.GetBlobClient($"{id}.xml");
            await blobClient.UploadAsync(stream);
        }
    }
}
