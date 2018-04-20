using AzureQuest.Api.Extensions;
using AzureQuest.Api.Model;
using AzureQuest.Api.Providers.Contracts;
using AzureQuest.Api.Repositories.Contracts;
using AzureQuest.Common;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureQuest.Api.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private ICosmosProvider _db;
        private INotificationRepository _notificationRepository;

        public TaskRepository(ICosmosProvider db, INotificationRepository notificationRepository)
        {
            this._db = db;
            this._notificationRepository = notificationRepository;
        }

        public OperationResultList<SimpleTask> SearchTasks(SimpleTaskQuery request = null)
        {
            var executionStart = DateTime.Now;
            request = request ?? new SimpleTaskQuery();
            request.MaxRecords = request.MaxRecords > 0 ? request.MaxRecords : 50;
            if(request.Order == null || request.Order.Count == 0) { request.Order = new List<DataOrderItem>() { new DataOrderItem("RegistrationDateTime", true) }; }

            var options = new FeedOptions() { MaxItemCount = request.MaxRecords };

            using (var client = _db.NewClient())
            {
                var query = _db.QueryTask(client, options).Where(t => t.Id != null);

                if (!string.IsNullOrEmpty(request.Text))
                {
                    query = query.Where(t =>
                        t.Title.ToLower().Contains(request.Text.ToLower()) ||
                        t.Description.ToLower().Contains(request.Text.ToLower())
                    );
                }

                var dataLength = query.Count();
                var results = query.OrderBy(request.Order).Take(request.MaxRecords).ToList();
                return new OperationResultList<SimpleTask>(results) { DataLength = dataLength, ExecutionTime = Convert.ToInt32(DateTime.Now.Subtract(executionStart).TotalMilliseconds) };
            }
        }

        public OperationResult<SimpleTask> GetTask(string id)
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

        public async Task<OperationResult> SaveTaskAsync(SimpleTask record)
        {
            var validation = record.Validate();
            if (!validation.IsValid) { return new OperationResult(false, validation.ResultsToHtml()); }
            var endpoint = UriFactory.CreateDocumentCollectionUri(_db.DatabaseName, _db.TaskCollectionName);

            using (var client = _db.NewClient())
            {
                ResourceResponse<Microsoft.Azure.Documents.Document> response;

                if (!string.IsNullOrEmpty(record.Id) && _db.DoesDocumentExists(_db.TaskCollectionName, record.Id))
                {
                    var task = _db.QueryTask(client).Where(t => t.Id == record.Id).AsEnumerable().FirstOrDefault();
                    if (task != null)
                    {
                        task.Title = record.Title;
                        task.Description = record.Description;
                        response = await client.UpsertDocumentAsync(endpoint, task, new RequestOptions());
                        return new OperationResult(true) { RecordId = record.Id, Tag = response };
                    }
                }

                record.Id = string.IsNullOrEmpty(record.Id) ? Guid.NewGuid().ToString() : record.Id;
                record.RegistrationDateTime = DateTime.Now;
                record.State = new List<SimpleTaskStatus>() {
                    new SimpleTaskStatus() {
                        Id = Guid.NewGuid().ToString(),
                        TaskId = record.Id,
                        Status = TaskStatusType.New,
                        RegistrationUser = record.User,
                        RegistrationDate = DateTime.Now
                    }
                };
                response = await client.CreateDocumentAsync(endpoint, record);
                return new OperationResult(true) { RecordId = record.Id };
            }
        }

        public async Task<OperationResult> UpdateTaskStatusAsync(SimpleTaskStatus record)
        {
            var validation = record.Validate();
            if (!validation.IsValid) { return new OperationResult(false, validation.ResultsToHtml()); }
            var endpoint = UriFactory.CreateDocumentCollectionUri(_db.DatabaseName, _db.TaskCollectionName);

            using (var client = _db.NewClient())
            {
                var task = _db.QueryTask(client).Where(t => t.Id == record.TaskId).AsEnumerable().FirstOrDefault();
                if (task != null)
                {
                    if (task.State == null || !task.State.Any(state => state.Status == record.Status))
                    {
                        task.State = task.State ?? new List<SimpleTaskStatus>();
                        task.State.Add(record);

                        var response = await client.UpsertDocumentAsync(endpoint, task);
                        await _notificationRepository.CreateNotificationFor(record);
                        return new OperationResult(true) { RecordId = task.Id };

                    }
                    else { return new OperationResult(false, "Task already contains the specified status type"); }
                }
                return new OperationResult(false, "Task not found");
            }
        }

        public async Task<OperationResult> RemoveTaskAsync(string id)
        {
            using (var client = _db.NewClient())
            {
                if (!string.IsNullOrEmpty(id) && _db.DoesDocumentExists(_db.TaskCollectionName, id))
                {
                    var response = await client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(_db.DatabaseName, _db.TaskCollectionName, id));
                    return new OperationResult(true) { Tag = response };
                }
            }
            return new OperationResult(false, "Task not found");
        }

       
    }
}
