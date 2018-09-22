using Deeproxio.Infrastructure;
using Deeproxio.Persistence.Context;
using Deeproxio.Persistence.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Deeproxio.AccountApi
{
    public class Startup : IServerStartup
    {
        public Startup(IHostingEnvironment env)
        {
	        var builder = new ConfigurationBuilder()
		        .SetBasePath(env.ContentRootPath)
		        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
		        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

	        builder.AddEnvironmentVariables();
	        Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
	        services.AddDbContext<DomainContext>(options =>
		        options.UseSqlServer(Configuration.GetConnectionString(nameof(DomainContext))));

			services.AddDbContext<IdentityDbContext>(options =>
		        options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

	        services.AddIdentity<ApplicationUser, IdentityRole>()
		        .AddEntityFrameworkStores<IdentityDbContext>()
		        .AddDefaultTokenProviders();

			services.AddMvc();

	        services.AddTransient<IEmailSender, AuthMessageSender>();
	        services.AddTransient<ISmsSender, AuthMessageSender>();
		}

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
				app.UseDeveloperExceptionPage();
	            app.UseDatabaseErrorPage();
			}
            else
            {
	            app.UseExceptionHandler("/Home/Error");
            }

	        app.UseStaticFiles();

	        app.UseAuthentication();
	        app.UseMvc(routes =>
	        {
		        routes.MapRoute(
			        name: "default",
			        template: "{controller=Values}/{action=Get}/{id?}");
	        });
		}
    }
}
