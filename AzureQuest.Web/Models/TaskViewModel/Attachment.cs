using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace AzureQuest.Web.Models.TaskViewModel
{
    public class Attachment
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [Required, StringLength(500)]
        public string Name { get; set; }

        [Required, StringLength(500)]
        public string Path { get; set; }

        public int Order { get; set; }
    }
}
