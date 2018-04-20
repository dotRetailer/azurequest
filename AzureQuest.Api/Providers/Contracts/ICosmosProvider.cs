using AzureQuest.Api.Model;
using Microsoft.Azure.Documents.Client;
using System.Linq;

namespace AzureQuest.Api.Providers.Contracts
{
    public interface ICosmosProvider: IBaseProvider
    {
        DocumentClient NewClient();
        string DatabaseName { get; }
        string TaskCollectionName { get; }
        string NotificationCollectionName { get; }
        bool DoesDocumentExists(string collectionName, string id);

        IOrderedQueryable<SimpleTask> QueryTask(DocumentClient client, FeedOptions options = null);
        IOrderedQueryable<Notification> QueryNotification(DocumentClient client, FeedOptions options = null);
    }
}
