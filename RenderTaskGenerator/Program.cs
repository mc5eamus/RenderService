using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RenderDomain.Config;
using RenderDomain.Contract;
using RenderDomain.Middleware;
using RenderDomain.Repository;
using RenderDomain.Services;
using RenderTaskGenerator;

IHostBuilder builder = Host.CreateDefaultBuilder(args).ConfigureServices(services =>
{
    services.AddOptions<BlobStorageRenderTaskRepoConfig>().BindConfiguration("BlobStorageRenderTaskRepo");
    services.AddOptions<CosmosRenderTaskRepoConfig>().BindConfiguration("CosmosRenderTaskRepo");
    services.AddOptions<OutboundServiceBusClientConfig>().BindConfiguration("OutboundServiceBusClient");
    services.AddSingleton<IRenderTaskFileRepository, BlobStorageRenderTaskRepository>();
    services.AddSingleton<IRenderTaskRepository, CosmosRenderTaskRepository>();
    services.AddSingleton<IRenderTaskOrchestrator, RenderTaskOrchestrator>();
    services.AddSingleton<IQueueSender, QueueSender>();
    services.AddSingleton<IRenderTaskOrchestrator, RenderTaskOrchestrator>();
    services.AddSingleton<RenderTaskFactory>();

});

using IHost host = builder.Build();

int num = args.Length>0 ? int.Parse(args[0]) : 1;

var factory = host.Services.GetRequiredService<RenderTaskFactory>();

//await factory.TestMe3();

await factory.Run(num);

