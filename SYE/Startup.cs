using System;
using System.Configuration;
using System.IO;
using GDSHelpers;
using GDSHelpers.Models.FormSchema;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Notify.Client;
using Notify.Interfaces;
using SYE.EsbWrappers;
using SYE.Helpers;
using SYE.Models.SubmissionSchema;
using SYE.Repository;
using SYE.Services;
using SYE.Services.Wrappers;
using SYE.ViewModels;

namespace SYE
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.ConfigureApplicationCookie(options => options.Cookie.SecurePolicy = CookieSecurePolicy.Always);


            services.AddMemoryCache();

            services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
                options.IdleTimeout = TimeSpan.FromMinutes(60);
                options.Cookie.IsEssential = true;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddHttpContextAccessor();
            services.AddOptions();

            services.Configure<ApplicationSettings>(Configuration.GetSection("ApplicationSettings"));

            services.TryAddScoped<ISessionService, SessionService>();

            services.TryAddSingleton<IGdsValidation, GdsValidation>();

            string notificationApiKey = Configuration.GetSection("ConnectionStrings:GovUkNotify").GetValue<String>("ApiKey");
            if (string.IsNullOrWhiteSpace(notificationApiKey))
            {
                throw new ConfigurationErrorsException($"Failed to load {nameof(notificationApiKey)} from application configuration.");
            }
            services.TryAddSingleton<IAsyncNotificationClient>(_ => new NotificationClient(notificationApiKey));
            services.TryAddScoped<INotificationService, NotificationService>();

            var searchConfiguration = Configuration.GetSection("ConnectionStrings:SearchDb").Get<SearchConfiguration>();
            if (searchConfiguration == null)
            {
                throw new ConfigurationErrorsException($"Failed to load {nameof(searchConfiguration)} from application configuration.");
            }
            services.TryAddSingleton<ICustomSearchIndexClient>(new CustomSearchIndexClient(searchConfiguration.SearchServiceName, searchConfiguration.IndexName, searchConfiguration.SearchApiKey));
            services.TryAddScoped<ISearchService, SearchService>();

            var cosmosDatabaseConnectionConfiguration = Configuration.GetSection("ConnectionStrings:DefaultCosmosDB").Get<CosmosConnection>();
            if (cosmosDatabaseConnectionConfiguration == null)
            {
                throw new ConfigurationErrorsException($"Failed to load {nameof(cosmosDatabaseConnectionConfiguration)} from application configuration.");
            }
            var cosmosDatabaseConnectionPolicy = Configuration.GetSection("CosmosDBConnectionPolicy").Get<ConnectionPolicy>() ?? ConnectionPolicy.Default;
            services.TryAddSingleton<IDocumentClient>(
                new DocumentClient(
                    new Uri(cosmosDatabaseConnectionConfiguration.Endpoint),
                    cosmosDatabaseConnectionConfiguration.Key,
                    cosmosDatabaseConnectionPolicy
                )
            );

            var formSchemaDatabase = Configuration.GetSection("CosmosDBCollections:FormSchemaDb").Get<AppConfiguration<FormVM>>();
            if (formSchemaDatabase == null)
            {
                throw new ConfigurationErrorsException($"Failed to load {nameof(formSchemaDatabase)} from application configuration.");
            }
            services.TryAddSingleton<IAppConfiguration<FormVM>>(formSchemaDatabase);

            var submissionsDatabase = Configuration.GetSection("CosmosDBCollections:SubmissionsDb").Get<AppConfiguration<SubmissionVM>>();
            if (submissionsDatabase == null)
            {
                throw new ConfigurationErrorsException($"Failed to load {nameof(submissionsDatabase)} from application configuration.");
            }
            services.TryAddSingleton<IAppConfiguration<SubmissionVM>>(submissionsDatabase);

            var configDatabase = Configuration.GetSection("CosmosDBCollections:ConfigDb").Get<AppConfiguration<ConfigVM>>();
            if (configDatabase == null)
            {
                throw new ConfigurationErrorsException($"Failed to load {nameof(configDatabase)} from application configuration.");
            }
            services.TryAddSingleton<IAppConfiguration<ConfigVM>>(configDatabase);

            var esbConfig = Configuration.GetSection("ConnectionStrings:EsbConfig").Get<EsbConfiguration<EsbConfig>>();
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

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Xss-Protection", "1");
                await next();
            });

            if (env.IsDevelopment() || env.IsLocal())
            {
                app.UseDeveloperExceptionPage();
                // Uncomment following to test error pages locally
                //app.UseStatusCodePagesWithReExecute("/Error/{0}");
            }
            else
            {
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseCookiePolicy();

            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}"
                );


                //routes.MapRoute("searchResults", "search/results/{search?}", defaults: new { controller = "Form", action = "SearchResults" });
                //routes.MapRoute("searchResults", "search/results/{search?}/{pageNo?}", defaults: new { controller = "Form", action = "SearchResults" });

                //routes.MapRoute("form", "form/{id?}", defaults: new { controller = "Form", action = "index" });
            });
        }
    }
}
