using System;
using GDSHelpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SYE.Repository;
using SYE.Services;
using SYE.Services.Wrappers;

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
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
                options.Secure = CookieSecurePolicy.SameAsRequest;
            });

            services.AddDistributedRedisCache(option =>
            {
                option.Configuration = Configuration.GetConnectionString("RedisCacheStore");
                option.InstanceName = "";
            });

            services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.IdleTimeout = TimeSpan.FromMinutes(60);
            });

            //TODO Get config settings here
            var searchServiceName = "sye-poc-azure-search";
            var apiKey = "260467EC7EE731A6CCC5CFDBD97D5D99";
            var indexName = "documentdb-index";

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            // TODO: Move all sensitive information to Azure Key Valut (using Managed Service Identity)
            var appConfig = Configuration.GetSection("ConnectionStrings").GetSection("SubmissionsDb").Get<AppConfiguration>();
            var connectionPolicy = Configuration.GetSection("CosmosDBConnectionPolicy").Get<ConnectionPolicy>();
            services.AddSingleton<IAppConfiguration>(appConfig);
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<ISubmissionService, SubmissionService>();
            services.AddScoped<IGdsValidation, GdsValidation>();
            services.AddScoped<IPageService, PageService>();
            services.AddScoped<ISearchService, SearchService>();
            services.AddScoped<ICustomSearchIndexClient, CustomSearchIndexClient>(c => new CustomSearchIndexClient(searchServiceName, indexName, apiKey));

            services.AddSingleton<IDocumentClient>(new DocumentClient(new Uri(appConfig.Endpoint), appConfig.Key, connectionPolicy, ConsistencyLevel.Strong));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment() || env.IsEnvironment("Local"))
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();

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
