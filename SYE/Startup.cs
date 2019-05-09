using System;
using GDSHelpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Notify.Interfaces;
using Notify.Client;
using SYE.Repository;
using SYE.Services;
using GDSHelpers.Models.FormSchema;
using SYE.Models.SubmissionSchema;
using SYE.Services.Wrappers;
using System.Collections.Generic;
using Newtonsoft.Json;
using SYE.EsbWrappers;

namespace SYE
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMemoryCache();

            services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
                options.IdleTimeout = TimeSpan.FromMinutes(60);
                options.Cookie.IsEssential = true;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            var connectionPolicy = Configuration.GetSection("CosmosDBConnectionPolicy").Get<ConnectionPolicy>();
            var formDatabaseConfig = Configuration.GetSection("ConnectionStrings").GetSection("FormSchemaDb").Get<AppConfiguration<FormVM>>();
            var searchConfig = Configuration.GetSection("ConnectionStrings").GetSection("SearchDb").Get<SearchConfiguration>();
            var submissionDatabaseConfig = Configuration.GetSection("ConnectionStrings").GetSection("SubmissionsDb").Get<AppConfiguration<SubmissionVM>>();
            var submissionConfig = Configuration.GetSection("ConnectionStrings").GetSection("ConfigDb").Get<AppConfiguration<ConfigVM>>();

            var indexClient = new CustomSearchIndexClient(searchConfig.SearchServiceName, searchConfig.IndexName, searchConfig.SearchApiKey);
            var searchService = new SearchService(indexClient);

            services.AddSingleton<IAppConfiguration<FormVM>>(formDatabaseConfig);
            services.AddSingleton<IAppConfiguration<SubmissionVM>>(submissionDatabaseConfig);
            services.AddSingleton<IAppConfiguration<ConfigVM>>(submissionConfig);

            //services.AddSingleton<ICosmosDocumentClient<FormVM>>(new CosmosDocumentClient<FormVM>() { Client = new DocumentClient(new Uri(formDatabaseConfig.Endpoint), formDatabaseConfig.Key, connectionPolicy) });
            //services.AddSingleton<ICosmosDocumentClient<SubmissionVM>>(new CosmosDocumentClient<SubmissionVM>() { Client = new DocumentClient(new Uri(submissionDatabaseConfig.Endpoint), submissionDatabaseConfig.Key, connectionPolicy) });
            services.AddSingleton<IDocumentClient>(new DocumentClient(new Uri(formDatabaseConfig.Endpoint), formDatabaseConfig.Key, connectionPolicy));

            //services.AddScoped(typeof(IGenericRepository<FormVM>), typeof(GenericRepository<FormVM>));
            //services.AddScoped(typeof(IGenericRepository<SubmissionVM>), typeof(GenericRepository<SubmissionVM>));
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            services.AddScoped<IGdsValidation, GdsValidation>();

            // TODO: Remove PageService, renamed to FormService
            services.AddScoped<IPageService, PageService>();
            services.AddScoped<ISearchService, SearchService>(s => searchService);
            services.AddScoped<ICustomSearchIndexClient, CustomSearchIndexClient>(c => indexClient);


            services.AddScoped<IFormService, FormService>();
            services.AddScoped<ISessionService, SessionService>();
            services.AddScoped<ISubmissionService, SubmissionService>();
            services.AddScoped<IDocumentService, DocumentService>();
            services.AddScoped<IEsbService, EsbService>();
            services.AddScoped<IEsbClient, EsbClient>();

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.TryAddSingleton<IGovUkNotifyConfiguration>(_ => Configuration.GetSection("GovUkNotify").Get<GovUkNotifyConfiguration>());
            services.TryAddSingleton<IAsyncNotificationClient>(_ => new NotificationClient(Configuration.GetConnectionString("GovUkNotifyApiKey")));
            services.TryAddScoped<INotificationService, NotificationService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)//, IApplicationLifetime lifetime, IDistributedCache cache)
        {
            if (env.IsDevelopment() || env.IsEnvironment("Local"))
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
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
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
