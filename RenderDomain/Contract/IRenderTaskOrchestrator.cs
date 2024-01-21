using RenderDomain.Model;

namespace RenderDomain.Contract
{
    /// <summary>
    /// Facade interface for managing render tasks
    /// </summary>
    public interface IRenderTaskOrchestrator
    {
        /// <summary>
        /// Creates a new render task pointing to the specified input file
        /// </summary>
        /// <param name="renderTask"></param>
        /// <param name="infile"></param>
        /// <returns></returns>
        Task<RenderTask> CreateRenderTaskAsync(RenderTask renderTask, Stream infile);
        /// <summary>
        /// Updates the status of a render task
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="status"></param>
        /// <param name="processingHost"></param>
        /// <returns></returns>
        Task<RenderTask> UpdateTaskStatus(string taskId, RenderTaskStatus status, string? processingHost);

        /// <summary>
        /// Completes a render task
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="result"></param>
        /// <param name="processingHost"></param>
        /// <returns></returns>
        Task CompleteTask(string taskId, Stream result, string? processingHost);

        /// <summary>
        /// Saves the results of a render task to file storage
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        Task SaveResults(string taskId, Stream result);
    }
}