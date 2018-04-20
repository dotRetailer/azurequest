using AzureQuest.Common;

namespace AzureQuest.Web.Models.TaskViewModel
{
    public class SimpleTaskQuery : BasePaginatedRequest
    {
        public string Text { get; set; }

    }
}
