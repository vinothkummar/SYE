using GDSHelpers;
using GDSHelpers.Models.FormSchema;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Notify.Client;
using Notify.Interfaces;
using SYE.EsbWrappers;
using SYE.Models.SubmissionSchema;
using SYE.Repository;
using SYE.Services;
using SYE.Services.Wrappers;
using SYE.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SYE.MiddlewareExtensions
{
    public static class ServiceConfiguration
    {
        public static void AddCustomServices(this IServiceCollection services, IConfiguration Config)
        {
            services.Configure<ApplicationSettings>(Config.GetSection("ApplicationSettings"));

            services.TryAddScoped<ISessionService, SessionService>();

            services.TryAddSingleton<IGdsValidation, GdsValidation>();
            
            string notificationApiKey = Config.GetSection("ConnectionStrings:GovUkNotify").GetValue<String>("ApiKey");
            if (string.IsNullOrWhiteSpace(notificationApiKey))
            {
                throw new ConfigurationErrorsException($"Failed to load {nameof(notificationApiKey)} from application configuration.");
            }
            services.TryAddSingleton<IAsyncNotificationClient>(_ => new NotificationClient(notificationApiKey));            
            services.TryAddScoped<INotificationService, NotificationService>();

            var searchConfiguration = Config.GetSection("ConnectionStrings:SearchDb").Get<SearchConfiguration>();
            if (searchConfiguration == null)
            {
                throw new ConfigurationErrorsException($"Failed to load {nameof(searchConfiguration)} from application configuration.");
            }
            services.TryAddSingleton<ICustomSearchIndexClient>(new CustomSearchIndexClient(searchConfiguration.SearchServiceName, searchConfiguration.IndexName, searchConfiguration.SearchApiKey));
            services.TryAddScoped<ISearchService, SearchService>();


            var cosmosDatabaseConnectionConfiguration = Config.GetSection("ConnectionStrings:DefaultCosmosDB").Get<CosmosConnection>();
            if (cosmosDatabaseConnectionConfiguration == null)
            {
                throw new ConfigurationErrorsException($"Failed to load {nameof(cosmosDatabaseConnectionConfiguration)} from application configuration.");
            }
            var cosmosDatabaseConnectionPolicy = Config.GetSection("CosmosDBConnectionPolicy").Get<ConnectionPolicy>() ?? ConnectionPolicy.Default;
            services.TryAddSingleton<IDocumentClient>(
                new DocumentClient(
                    new Uri(cosmosDatabaseConnectionConfiguration.Endpoint),
                    cosmosDatabaseConnectionConfiguration.Key,
                    cosmosDatabaseConnectionPolicy
                )
            );

            var formSchemaDatabase = Config.GetSection("CosmosDBCollections:FormSchemaDb").Get<AppConfiguration<FormVM>>();
            if (formSchemaDatabase == null)
            {
                throw new ConfigurationErrorsException($"Failed to load {nameof(formSchemaDatabase)} from application configuration.");
            }
            services.TryAddSingleton<IAppConfiguration<FormVM>>(formSchemaDatabase);

            var submissionsDatabase = Config.GetSection("CosmosDBCollections:SubmissionsDb").Get<AppConfiguration<SubmissionVM>>();
            if (submissionsDatabase == null)
            {
                throw new ConfigurationErrorsException($"Failed to load {nameof(submissionsDatabase)} from application configuration.");
            }
            services.TryAddSingleton<IAppConfiguration<SubmissionVM>>(submissionsDatabase);

            var configDatabase = Config.GetSection("CosmosDBCollections:ConfigDb").Get<AppConfiguration<ConfigVM>>();
            if (configDatabase == null)
            {
                throw new ConfigurationErrorsException($"Failed to load {nameof(configDatabase)} from application configuration.");
            }
            services.TryAddSingleton<IAppConfiguration<ConfigVM>>(configDatabase);

            var esbConfig = Config.GetSection("ConnectionStrings:EsbConfig").Get<EsbConfiguration<EsbConfig>>();
            if (esbConfig == null)
            {
                throw new ConfigurationErrorsException($"Failed to load {nameof(esbConfig)} from application configuration.");
            }
            services.AddSingleton<IEsbConfiguration<EsbConfig>>(esbConfig);

            IFileProvider physicalProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
            services.AddSingleton<IFileProvider>(physicalProvider);

            services.TryAddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.TryAddScoped<IEsbClient, EsbClient>();
            services.TryAddScoped<IEsbWrapper, EsbWrapper>();
            services.TryAddScoped<IEsbService, EsbService>();
            services.TryAddScoped<IFormService, FormService>();
            services.TryAddScoped<ISubmissionService, SubmissionService>();
            services.TryAddScoped<IDocumentService, DocumentService>();
        }
    }
}
