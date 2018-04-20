using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace AzureQuest.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                //.UseApplicationInsights()
                //.ConfigureAppConfiguration(VaultConfiguration)
                .UseStartup<Startup>()
                .Build();

        static void VaultConfiguration(WebHostBuilderContext webHostBuilderContext, IConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("azurekeyvault.json", false, true)
                .AddEnvironmentVariables();

            var config = configurationBuilder.Build();

            configurationBuilder.AddAzureKeyVault(
                $"https://{config["azureKeyVault:vault"]}.vault.azure.net/",
                config["azureKeyVault:clientId"],
                config["azureKeyVault:clientSecret"]
            );
        }
    }
}
