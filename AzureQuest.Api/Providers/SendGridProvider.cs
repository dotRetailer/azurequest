using AzureQuest.Api.Providers.Contracts;
using AzureQuest.Common;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureQuest.Api.Providers
{
    public class SendGridProvider : IEmailProvider
    {
        protected string APIKey { get; set; }

        public SendGridProvider(IConfiguration configuration)
        {
            this.APIKey = configuration["appSettings:apiKeys:sendGrid"];
        }
        
        public async Task<OperationResult> SendEmailAsync(string email, string subject, string message)
        {
            return await Execute(this.APIKey, subject, message, email);
        }

        public async Task<OperationResult> SendEmailAsync(string email, string subject, string templateId, Dictionary<string, string> arguments)
        {
            return await Execute(this.APIKey, subject, email, templateId, arguments);
        }

        public async Task<OperationResult> Execute(string apiKey, string subject, string email, string templateId = null, Dictionary<string, string> arguments = null)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                // should be a domain other than yahoo.com, outlook.com, hotmail.com, gmail.com
                From = new EmailAddress("donotreply@somewhere.com", "Azure Quest Workshop"),
                Subject = subject,
                TemplateId = templateId
            };

            msg.AddSubstitution("<%subject%>", subject);

            if(arguments != null) { arguments.ToList().ForEach(pair => { msg.AddSubstitution(pair.Key, pair.Value); }); }

            msg.AddTo(new EmailAddress(email));
            msg.SetOpenTracking(true, "<%tracking%>");
            msg.SetClickTracking(true, true);
            msg.SetSubscriptionTracking(true);

            var response = await client.SendEmailAsync(msg);
            return new OperationResult(true) { Tag = response };
        }

    }
}
