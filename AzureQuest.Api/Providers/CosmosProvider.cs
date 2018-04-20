using AzureQuest.Api.Model;
using AzureQuest.Api.Providers.Contracts;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace AzureQuest.Api.Providers
{
    public class CosmosProvider : ICosmosProvider
    {
        private CosmosConnectionData _connection;
        public string DatabaseName { get { return "AzureQuest"; } }
        public string TaskCollectionName { get { return "Tasks"; } }
        public string NotificationCollectionName { get { return "Notifications"; } }

        public CosmosProvider(IOptions<CosmosConnectionData> connection)
        {
            _connection = connection.Value;
        }

        public DocumentClient NewClient()
        {
            return new DocumentClient(
                new Uri(_connection.Endpoint), _connection.Key);
        }

        public bool DoesDocumentExists(string collectionName, string id)
        {
            using(var client = this.NewClient())
            {
                var uri = UriFactory.CreateDocumentCollectionUri(this.DatabaseName, collectionName);
                var query = client.CreateDocumentQuery<Microsoft.Azure.Documents.Document>(uri, new FeedOptions() { MaxItemCount = 1 });
                return query.Where(x => x.Id == id).Select(x => x.Id).AsEnumerable().Any();
            }
        }

        public IOrderedQueryable<SimpleTask> QueryTask(DocumentClient client, FeedOptions options = null)
        {
            options = options ?? new FeedOptions() { MaxItemCount = 1 };
            return client.CreateDocumentQuery<SimpleTask>(
                UriFactory.CreateDocumentCollectionUri(this.DatabaseName, this.TaskCollectionName), options);
        }

        public IOrderedQueryable<Notification> QueryNotification(DocumentClient client, FeedOptions options = null)
        {
            options = options ?? new FeedOptions() { MaxItemCount = 1 };
            return client.CreateDocumentQuery<Notification>(
                UriFactory.CreateDocumentCollectionUri(this.DatabaseName, this.NotificationCollectionName), options);
        }
    }

    public class CosmosConnectionData
    {
        public string Endpoint { get; set; }
        public string Key { get; set; }
    }

}
