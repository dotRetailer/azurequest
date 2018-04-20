using System;
using System.Collections.Generic;
using System.Text;

namespace AzureQuest.Common
{
    public class NotificationQueueItem
    {
        public NotificationQueueItem() { }

        public NotificationQueueItem(string id) { this.Id = id; }

        public string Id { get; set; }
    }
}
