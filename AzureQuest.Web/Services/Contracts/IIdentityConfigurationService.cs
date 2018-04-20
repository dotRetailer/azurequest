using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AzureQuest.Web.Services.Contracts
{
    public interface IIdentityConfigurationService
    {
        void ConfigureIdentity(IServiceCollection services, IConfiguration configuration);

        void ConfigureIdentityAuth0(IServiceCollection services, IConfiguration configuration);
    }
}
