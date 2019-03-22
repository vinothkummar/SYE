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
using GDSHelpers.Models.SubmissionSchema;
using GDSHelpers.Models.FormSchema;

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
            });

            services.AddMemoryCache();

            services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
                options.IdleTimeout = TimeSpan.FromMinutes(60);
                options.Cookie.IsEssential = true;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // TODO: Move all sensitive information out of configuration files to a safer place

            var connectionPolicy = Configuration.GetSection("CosmosDBConnectionPolicy").Get<ConnectionPolicy>();
            var formDatabaseConfig = Configuration.GetSection("ConnectionStrings").GetSection("FormSchemaDb").Get<AppConfiguration<FormVM>>();
            var submissionDatabaseConfig = Configuration.GetSection("ConnectionStrings").GetSection("SubmissionsDb").Get<AppConfiguration<SubmissionVM>>();

            services.AddSingleton<IAppConfiguration<FormVM>>(formDatabaseConfig);
            services.AddSingleton<IAppConfiguration<SubmissionVM>>(submissionDatabaseConfig);

            services.AddSingleton<ICosmosDocumentClient<FormVM>>(new CosmosDocumentClient<FormVM>() { Client = new DocumentClient(new Uri(formDatabaseConfig.Endpoint), formDatabaseConfig.Key, connectionPolicy) });
            services.AddSingleton<ICosmosDocumentClient<SubmissionVM>>(new CosmosDocumentClient<SubmissionVM>() { Client = new DocumentClient(new Uri(submissionDatabaseConfig.Endpoint), submissionDatabaseConfig.Key, connectionPolicy) });

            services.AddScoped(typeof(IGenericRepository<FormVM>), typeof(GenericRepository<FormVM>));
            services.AddScoped(typeof(IGenericRepository<SubmissionVM>), typeof(GenericRepository<SubmissionVM>));

            services.AddScoped<IGdsValidation, GdsValidation>();
            services.AddScoped<IPageService, PageService>();
            services.AddScoped<ISubmissionService, SubmissionService>();
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
