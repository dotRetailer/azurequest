using AzureQuest.Common;

namespace AzureQuest.Api.Model
{
    public class SimpleTaskQuery : BasePaginatedRequest
    {
        public string Text { get; set; }

    }
}
