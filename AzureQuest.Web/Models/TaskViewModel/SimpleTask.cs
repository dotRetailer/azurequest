using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AzureQuest.Web.Models.TaskViewModel
{
    public class SimpleTask
    {
        [JsonProperty(PropertyName = "id")]
        [Display(Name = "Id")]
        public string Id { get; set; }

        [Required, StringLength(1000, MinimumLength = 3)]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required, StringLength(9000, MinimumLength = 3)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Registration Date")]
        public DateTime RegistrationDateTime { get; set; }

        [Display(Name = "User")]
        public SimpleUser User { get; set; }

        [Display(Name = "Status")]
        public List<SimpleTaskStatus> State { get; set; }

        [Display(Name = "Attachments")]
        public List<Attachment> Attachments { get; set; }                
    }
}
