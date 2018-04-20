using AzureQuest.Web.Data;
using AzureQuest.Web.Services;
using AzureQuest.Web.Services.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AzureQuest.Web
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.ConfigureIdentity(Configuration);
            //services.ConfigureIdentityAuth0(Configuration);
            //services.Configure<Auth0Settings>(Configuration.GetSection("auth0"));

            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMemoryCache();
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseExceptionHandler();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseMiddleware<CustomExceptionHandlerMiddleware>();
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
                        
        }

    }
}
