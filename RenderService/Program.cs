using RenderDomain.Config;
using RenderDomain.Contract;
using RenderDomain.Middleware;
using RenderDomain.Repository;
using RenderDomain.Services;
using RenderService.Middleware;
using RenderService.Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddOptions<BlobStorageRenderTaskRepoConfig>().BindConfiguration("BlobStorageRenderTaskRepo");
builder.Services.AddOptions<CosmosRenderTaskRepoConfig>().BindConfiguration("CosmosRenderTaskRepo");
builder.Services.AddOptions<InboundServiceBusClientConfig>().BindConfiguration("InboundServiceBusClient");
builder.Services.AddOptions<OutboundServiceBusClientConfig>().BindConfiguration("OutboundServiceBusClient");
builder.Services.AddSingleton<IQueueConsumer, QueueConsumer>();
builder.Services.AddSingleton<IQueueSender, QueueSender>();
builder.Services.AddSingleton<IRenderTaskFileRepository, BlobStorageRenderTaskRepository>();
builder.Services.AddSingleton<IRenderTaskRepository, CosmosRenderTaskRepository>();
builder.Services.AddSingleton<IRenderTaskOrchestrator, RenderTaskOrchestrator>();
builder.Services.AddHostedService<RenderWorker>();
builder.Services.AddHostedService<PingWorker>();

var host = builder.Build();
host.Run();
