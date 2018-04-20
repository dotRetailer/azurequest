using AzureQuest.Api.Extensions;
using AzureQuest.Api.Model;
using AzureQuest.Api.Providers.Contracts;
using AzureQuest.Api.Repositories.Contracts;
using AzureQuest.Common;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureQuest.Api.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private IConfiguration _config;
        private ICosmosProvider _db;
        private IEmailProvider _emailProvider;

        public NotificationRepository(IConfiguration configuration, ICosmosProvider db, IEmailProvider emailProvider)
        {
            this._db = db;
            this._config = configuration;
            this._emailProvider = emailProvider;
        }

        public async Task<OperationResult> SendNotificationEmail(string id)
        {
            if (string.IsNullOrEmpty(id)) { return new OperationResult(false, "Notification ID not identified"); }
            var notification = GetNotification(id);
            if(notification.Data != null)
            {
                if (notification.Data.Sent) { await DeQueueNotificationMessage(notification.Data.Id); return new OperationResult(true); }

                var task = GetTask(notification.Data.TaskId);
                var status = task?.Data.State.FirstOrDefault(item => item.Id == notification.Data.TaskStatusId);
                if (task != null && status != null)
                {
                    var arguments = new Dictionary<string, string>();
                    arguments.Add("<%user%>", notification.Data.User.Name);
                    arguments.Add("<%task-title%>", task.Data.Title);
                    arguments.Add("<%task-date%>", status.RegistrationDate.ToLongDateString());
                    arguments.Add("<%task-status%>", status.Status.ToString());
                    var response = await _emailProvider.SendEmailAsync(notification.Data.User.Email, "Task Status Changed - AzureQuest", "{MyTransactionalTemplateId}", arguments);

                    if (response.Success)
                    {
                        notification.Data.Sent = true;
                        notification.Data.DeliveryDate = DateTime.Now;
                        notification.Data.DeliveryReponse = response.Tag;
                        await SaveNotificationAsync(notification.Data);
                        await DeQueueNotificationMessage(notification.Data.Id);
                        return new OperationResult(true);
                    }
                }
            }
            return new OperationResult(false, "Notification not found");
        }

        public async Task<OperationResult> CreateNotificationFor(SimpleTaskStatus status)
        {
            var users = new List<SimpleUser>();
            var task = GetTask(status.TaskId).Data;
            if(task?.State.Count > 1) { task.State.ForEach(item => { if (!users.Any(u => u.Id == item.RegistrationUser.Id)) { users.Add(item.RegistrationUser); } }); }
            else { users.Add(status.RegistrationUser); }

            foreach (var user in users)
            {
                var notification = new Notification()
                {
                    Id = Guid.NewGuid().ToString(),
                    User = user,
                    Destination = user.Email,
                    RegistrationDate = DateTime.Now,
                    Subject = "Task Change Notification",
                    TaskId = status.TaskId,
                    TaskStatusId = status.Id,
                    Message = "<%Notification Template%>"
                };
                var res = await SaveNotificationAsync(notification);
                if (res.Success) { await QueueNotificationMessage(notification.Id); }
            }
            return new OperationResult(true);
        }

        public OperationResultList<Notification> SearchNotifications()
        {
            var executionStart = DateTime.Now;
            var options = new FeedOptions() { MaxItemCount = 50 };

            using (var client = _db.NewClient())
            {
                var query = _db.QueryNotification(client, options).Where(t => t.Id != null);
                var dataLength = query.Count();
                var results = query.OrderByDescending(n => n.RegistrationDate).Take(50).ToList();
                return new OperationResultList<Notification>(results) { DataLength = dataLength, ExecutionTime = Convert.ToInt32(DateTime.Now.Subtract(executionStart).TotalMilliseconds) };
            }
        }

        public OperationResult<Notification> GetNotification(string id)
        {
            using (var client = _db.NewClient())
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var notification = _db.QueryNotification(client).Where(t => t.Id == id).AsEnumerable().FirstOrDefault();
                    if (notification != null)
                    {
                        return new OperationResult<Notification>(notification);
                    }
                }
                return new OperationResult<Notification>(false, "Notification not found");
            }
        }

        public async Task<OperationResult> SaveNotificationAsync(Notification record)
        {
            var validation = record.Validate();
            if (!validation.IsValid) { return new OperationResult(false, validation.ResultsToHtml()); }
            var endpoint = UriFactory.CreateDocumentCollectionUri(_db.DatabaseName, _db.NotificationCollectionName);

            using (var client = _db.NewClient())
            {
                ResourceResponse<Microsoft.Azure.Documents.Document> response;

                if (!string.IsNullOrEmpty(record.Id) && _db.DoesDocumentExists(_db.NotificationCollectionName, record.Id))
                {
                    var notification = _db.QueryNotification(client).Where(t => t.Id == record.Id).AsEnumerable().FirstOrDefault();
                    if (notification != null)
                    {
                        notification.DeliveryDate = record.DeliveryDate;
                        notification.DeliveryReponse = record.DeliveryReponse;
                        notification.Destination = record.Destination;
                        notification.Message = record.Message;
                        notification.Sent = record.Sent;
                        notification.Subject = record.Subject;
                        response = await client.UpsertDocumentAsync(endpoint, notification, new RequestOptions());
                        return new OperationResult(true) { RecordId = record.Id, Tag = response };
                    }
                }

                record.Id = string.IsNullOrEmpty(record.Id) ? Guid.NewGuid().ToString() : record.Id;
                record.RegistrationDate = DateTime.Now;

                response = await client.CreateDocumentAsync(endpoint, record);
                return new OperationResult(true) { RecordId = record.Id, Tag = response };
            }
        }

        public async Task<OperationResult> RemoveNotificationAsync(string id)
        {
            using (var client = _db.NewClient())
            {
                if (!string.IsNullOrEmpty(id) && _db.DoesDocumentExists(_db.NotificationCollectionName, id))
                {
                    var response = await client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(_db.DatabaseName, _db.NotificationCollectionName, id));
                    return new OperationResult(true) { Tag = response };
                }
            }
            return new OperationResult(false, "Notification not found");
        }

        protected OperationResult<SimpleTask> GetTask(string id)
        {
            using (var client = _db.NewClient())
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var task = _db.QueryTask(client).Where(t => t.Id == id).AsEnumerable().FirstOrDefault();
                    if (task != null)
                    {
                        return new OperationResult<SimpleTask>(task);
                    }
                }
                return new OperationResult<SimpleTask>(false, "Task not found");
            }
        }
        
        protected async Task<OperationResult> QueueNotificationMessage(string id)
        {
            var connectionString = _config["appSettings:apiKeys:azureQuestStorage"];
            var queue = CloudStorageAccount.Parse(connectionString).CreateCloudQueueClient().GetQueueReference("{MyStorageQueueName}");
            await queue.AddMessageAsync(new CloudQueueMessage(Newtonsoft.Json.JsonConvert.SerializeObject(new NotificationQueueItem(id))));
            return new OperationResult(true);
        }

        protected async Task<OperationResult> DeQueueNotificationMessage(string id)
        {
            var connectionString = _config["appSettings:apiKeys:azureQuestStorage"];
            var queue = CloudStorageAccount.Parse(connectionString).CreateCloudQueueClient().GetQueueReference("{MyStorageQueueName}");
            await queue.DeleteMessageAsync(new CloudQueueMessage(Newtonsoft.Json.JsonConvert.SerializeObject(new NotificationQueueItem(id))));
            return new OperationResult(true);
        }

    }
}
