using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;
using SYE.Helpers;

namespace SYE
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .CaptureStartupErrors(true)
                .SuppressStatusMessages(false)
                .ConfigureAppConfiguration((builderContext, configurationBuilder) =>
                {
                    if (builderContext.HostingEnvironment.IsLocal())
                    {
                        configurationBuilder.AddUserSecrets<Startup>();
                    }
                    else
                    {
                        var builtConfig = configurationBuilder.Build();
                        string keyVaultEndpoint = builtConfig?.GetValue<string>("KeyVaultName");
                        if (!string.IsNullOrWhiteSpace(keyVaultEndpoint))
                        {
                            var azureServiceTokenProvider = new AzureServiceTokenProvider();
                            var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
                            configurationBuilder.AddAzureKeyVault(keyVaultEndpoint, keyVaultClient, new DefaultKeyVaultSecretManager());
                        }
                    }
                })
                .UseApplicationInsights()
                .UseStartup<Startup>()
                .ConfigureLogging((builderContext, loggingBuilder) =>
                {
                    //loggingBuilder.ClearProviders();
                    if (!builderContext.HostingEnvironment.IsLocal())
                    {
                        loggingBuilder.AddApplicationInsights();
                    }
                });
        }
    }
}
