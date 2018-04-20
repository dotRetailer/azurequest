using AzureQuest.Web.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace AzureQuest.Web.Services
{
    public static class IdentityExtensions
    {
        public static void ConfigureIdentity(this IServiceCollection services, IConfiguration configuration)
        {
            new IdentityConfigurationService().ConfigureIdentity(services, configuration);
        }

        public static void ConfigureIdentityAuth0(this IServiceCollection services, IConfiguration configuration)
        {
            new IdentityConfigurationService().ConfigureIdentityAuth0(services, configuration);
        }

        public static void IncludeEnvironmentVariables(this IConfigurationRoot config)
        {
            config.AsEnumerable().ToList().ForEach(item =>
            {
                if (!string.IsNullOrEmpty(item.Key) && !string.IsNullOrEmpty(item.Value))
                    Environment.SetEnvironmentVariable(item.Key, item.Value);
            });
        }
    }
}
