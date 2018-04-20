using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AzureQuest.Api.Model
{
    public class SimpleTask
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [Required, StringLength(1000)]
        public string Title { get; set; }

        [Required, StringLength(9000)]
        public string Description { get; set; }
                
        public SimpleUser User { get; set; }
        
        public DateTime? RegistrationDateTime { get; set; }

        public List<SimpleTaskStatus> State { get; set; }

        public List<Attachment> Attachments { get; set; }                
    }
}
