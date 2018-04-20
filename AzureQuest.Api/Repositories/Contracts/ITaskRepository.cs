using AzureQuest.Api.Model;
using AzureQuest.Common;
using System.Threading.Tasks;

namespace AzureQuest.Api.Repositories.Contracts
{
    public interface ITaskRepository : IBaseRepository
    {
        OperationResultList<SimpleTask> SearchTasks(SimpleTaskQuery request = null);

        OperationResult<SimpleTask> GetTask(string id);

        Task<OperationResult> SaveTaskAsync(SimpleTask record);

        Task<OperationResult> UpdateTaskStatusAsync(SimpleTaskStatus record);

        Task<OperationResult> RemoveTaskAsync(string id);

    }
}
