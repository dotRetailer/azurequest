using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace AzureQuest.Web.Models.TaskViewModel
{
    public class Notification
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [Required]
        public string TaskId { get; set; }

        public string TaskStatusId { get; set; }

        public SimpleUser User { get; set; }

        [Required, StringLength(500)]
        public string Destination { get; set; }

        [Required, StringLength(500)]
        public string Subject { get; set; }

        [Required]
        public string Message { get; set; }

        public bool Sent { get; set; }

        public object DeliveryReponse { get; set; }

        public DateTime DeliveryDate { get; set; }

        public DateTime RegistrationDate { get; set; }

    }
}
