using RenderDomain.Config;
using RenderDomain.Contract;
using RenderDomain.Middleware;
using RenderDomain.Repository;
using RenderDomain.Services;
using RenderService.Middleware;
using RenderService.Services;

namespace RenderService.Extensions
{
    public static class ServiceConfig
    {
        // Implements service configuration for RenderService as an IServiceCollection extension method
        public static void AddRenderServices(this IServiceCollection services)
        {
            services.AddOptions<BlobStorageRenderTaskRepoConfig>().BindConfiguration("BlobStorageRenderTaskRepo");
            services.AddOptions<CosmosRenderTaskRepoConfig>().BindConfiguration("CosmosRenderTaskRepo");
            services.AddOptions<InboundServiceBusClientConfig>().BindConfiguration("InboundServiceBusClient");
            services.AddOptions<OutboundServiceBusClientConfig>().BindConfiguration("OutboundServiceBusClient");
            services.AddSingleton<IQueueConsumer, QueueConsumer>();
            services.AddSingleton<IQueueSender, QueueSender>();
            services.AddSingleton<IRenderTaskFileRepository, BlobStorageRenderTaskRepository>();
            services.AddSingleton<IRenderTaskRepository, CosmosRenderTaskRepository>();
            services.AddSingleton<IRenderTaskOrchestrator, RenderTaskOrchestrator>();
            services.AddHostedService<RenderWorker>();
            services.AddHostedService<PingWorker>();
        }
    }
}
