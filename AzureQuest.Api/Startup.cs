using AzureQuest.Api.Providers;
using AzureQuest.Api.Providers.Contracts;
using AzureQuest.Api.Repositories;
using AzureQuest.Api.Repositories.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System.Collections.Generic;

namespace AzureQuest.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CosmosConnectionData>(Configuration.GetSection("Cosmos"));

            services.AddSingleton<ICosmosProvider, CosmosProvider>();
            services.AddSingleton<IEmailProvider, SendGridProvider>();

            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();

            services.AddMvc().AddJsonOptions(options => { options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented; });
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new Info { Title = "AzureQuest API", Version = "v1" }); });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });
            
            services.Configure<MvcOptions>(options => { options.Filters.Add(new CorsAuthorizationFilterFactory("AllowAllOrigins")); });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.Authority = "https://{MyAuth0ApiProjectName}.eu.auth0.com/";
                options.Audience = "{MyAuth0ApiIdentifier}";
            });

            //services.AddApplicationInsightsTelemetry();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseSwagger();
            app.UseSwaggerUI(config => { config.SwaggerEndpoint("/swagger/v1/swagger.json", "AzureQuest API V1"); });
            
            app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
