using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace AzureQuest.Web.Models.TaskViewModel
{
    public enum TaskStatusType { Unknown = 0, New = 1, Canceled = 2, Done = 3 }

    public class SimpleTaskStatus
    {

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [Required]
        public string TaskId { get; set; }

        public TaskStatusType Status { get; set; }

        public DateTime RegistrationDate { get; set; }

        public SimpleUser RegistrationUser { get; set; }
    }
}
