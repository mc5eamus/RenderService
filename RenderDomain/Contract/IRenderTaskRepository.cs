using RenderDomain.Model;

namespace RenderDomain.Contract
{
    /// <summary>
    /// Describes a simple repository of rendering tasks with a few basic CRUD operations.
    /// For now, it lacks the ability to query for tasks, while only offering point reads and writes.
    /// </summary>
    public interface IRenderTaskRepository
    {
        /// <summary>
        /// Point read for a task given its id 
        /// </summary>
        /// <param name="id">Task id</param>
        /// <returns></returns>
        Task<RenderTask> GetRenderTaskAsync(string id);
        /// <summary>
        /// Persists a task in the repository
        /// </summary>
        /// <param name="renderTask">Task object</param>
        /// <returns>Task or null if the task witht the specified id cannot be found</returns>
        Task<RenderTask> CreateRenderTaskAsync(RenderTask renderTask);
        /// <summary>
        /// Updates a task in the repository with the new value
        /// </summary>
        /// <param name="renderTask">Task object</param>
        /// <returns>Echoed task</returns>
        Task<RenderTask> UpdateRenderTaskAsync(RenderTask renderTask);
        /// <summary>
        /// Deletes a task from the repository
        /// </summary>
        /// <param name="id">Task id</param>
        /// <returns>Echoed task</returns>
        Task DeleteRenderTaskAsync(string id);
    }
}
