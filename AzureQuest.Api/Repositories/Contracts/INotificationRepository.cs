using AzureQuest.Api.Model;
using AzureQuest.Common;
using System.Threading.Tasks;

namespace AzureQuest.Api.Repositories.Contracts
{
    public interface INotificationRepository : IBaseRepository
    {
        Task<OperationResult> SendNotificationEmail(string id);

        Task<OperationResult> CreateNotificationFor(SimpleTaskStatus status);

        OperationResultList<Notification> SearchNotifications();

        OperationResult<Notification> GetNotification(string id);

        Task<OperationResult> SaveNotificationAsync(Notification record);

        Task<OperationResult> RemoveNotificationAsync(string id);

    }

}
