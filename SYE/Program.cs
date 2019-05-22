using System;
using System.Configuration;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.Logging;

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
                //.ConfigureAppConfiguration((hostingContext, configurationBuilder) =>
                //{
                //    if (!hostingContext.HostingEnvironment.IsEnvironment("Local1"))
                //    {
                //        var builtConfig = configurationBuilder.Build();
                //        string keyVaultEndpoint = builtConfig?.GetValue<string>("KeyVaultName");
                //        if (string.IsNullOrWhiteSpace(keyVaultEndpoint))
                //        {
                //            throw new ConfigurationErrorsException($"Failed to load {nameof(keyVaultEndpoint)} from application configuration.");
                //        }
                //        var azureServiceTokenProvider = new AzureServiceTokenProvider();
                //        var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
                //        configurationBuilder.AddAzureKeyVault(keyVaultEndpoint, keyVaultClient, new DefaultKeyVaultSecretManager());
                //    }
                //})
                .UseApplicationInsights()
                .UseStartup<Startup>()
                .ConfigureLogging((hostingContext, logBuilder) =>
                {
                    //logBuilder.ClearProviders();
                    //if (!hostingContext.HostingEnvironment.IsEnvironment("Local"))
                    //{
                    //    logBuilder.AddApplicationInsights();
                    //}
                    //else
                    //{
                    //    logBuilder.AddDebug();
                    //    logBuilder.AddConsole();
                    //}
                    logBuilder.AddApplicationInsights();
                });
        }
    }
}
