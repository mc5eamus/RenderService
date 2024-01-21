using RenderDomain.Contract;
using RenderDomain.Model;

namespace RenderDomain.Services
{
    /// <summary>
    /// Orchestrates the creation and update of render tasks as well as related messages
    /// </summary>
    public class RenderTaskOrchestrator : IRenderTaskOrchestrator
    {
        private readonly IRenderTaskRepository taskRepo;
        private readonly IRenderTaskFileRepository taskFileRepo;

        public RenderTaskOrchestrator(
            IRenderTaskRepository taskRepo,
            IRenderTaskFileRepository taskFileRepo)
        {
            this.taskRepo = taskRepo;
            this.taskFileRepo = taskFileRepo;
        }

        public async Task<RenderTask> CreateRenderTaskAsync(RenderTask renderTask, Stream infile)
        {
            var task = await taskRepo.CreateRenderTaskAsync(renderTask);
            await taskFileRepo.SaveRenderTaskInput(task.Id, infile);
            return task;
        }

        public async Task<RenderTask> UpdateTaskStatus(string taskId, RenderTaskStatus status, string? processingHost)
        {
            var task = await taskRepo.GetRenderTaskAsync(taskId);
            task.Status = status;
            task.ProcessingHost = processingHost;
            task.Timestamp = DateTime.UtcNow;
            return await taskRepo.UpdateRenderTaskAsync(task);
        }

        public async Task CompleteTask(string taskId, Stream result, string? processingHost)
        {
            await taskFileRepo.SaveRenderTaskOutput(taskId, result);
            var task = await taskRepo.GetRenderTaskAsync(taskId);
            task.Status = RenderTaskStatus.Complete;
            task.ProcessingHost = processingHost;
            task.Timestamp = DateTime.UtcNow;
            await taskRepo.UpdateRenderTaskAsync(task);
        }

        public async Task SaveResults(string taskId, Stream result)
        {
            await taskFileRepo.SaveRenderTaskOutput(taskId, result);
        }
    }
}
