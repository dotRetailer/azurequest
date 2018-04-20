using AzureQuest.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureQuest.Api.Providers.Contracts
{
    public interface IEmailProvider : IBaseProvider
    {
        Task<OperationResult> SendEmailAsync(string email, string subject, string message);

        Task<OperationResult> SendEmailAsync(string email, string subject, string templateId = null, Dictionary<string, string> arguments = null);
    }
}
