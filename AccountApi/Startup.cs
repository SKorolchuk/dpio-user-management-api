using System.Net;
using AutoMapper;
using Deeproxio.AccountApi.Extensions;
using Deeproxio.Infrastructure.Notification;
using Deeproxio.Infrastructure.Runtime;
using Deeproxio.Persistence.Identity.Context;
using Deeproxio.Persistence.Identity.Identity;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Prometheus;

namespace Deeproxio.AccountApi
{
    public class Startup : IServerStartup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString(nameof(IdentityDbContext));
            var migrationsAssembly = typeof(IdentityDbContext).Assembly.FullName;

            services.AddDbContext<IdentityDbContext, IdentityDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString(nameof(IdentityDbContext)),
                    b => b.MigrationsAssembly(typeof(IdentityDbContext).Assembly.FullName)));

            services.TryAddTransient<IHttpContextAccessor, HttpContextAccessor>();

            services.AddIdentity<ApplicationUser, ApplicationRole>(o =>
            {
                // configure identity options
                o.Password.RequireDigit = false;
                o.Password.RequireLowercase = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequiredLength = 6;
            }).AddEntityFrameworkStores<IdentityDbContext>()
                .AddDefaultTokenProviders();

            var identityServerBuilder = services.AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;
                })
                // this adds the config data from DB (clients, resources)
                .AddConfigurationStore(options =>
                {
                    options.DefaultSchema = "dbo";
                    options.ConfigureDbContext = b =>
                        b.UseNpgsql(connectionString,
                            sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                // this adds the operational data from DB (codes, tokens, consents)
                .AddOperationalStore(options =>
                {
                    options.DefaultSchema = "dbo";
                    options.ConfigureDbContext = b =>
                        b.UseNpgsql(connectionString,
                            sql => sql.MigrationsAssembly(migrationsAssembly));

                    // this enables automatic token cleanup. this is optional.
                    options.EnableTokenCleanup = true;
                })
                .AddAspNetIdentity<ApplicationUser>();

            services.AddAutoMapper();
            services.AddCors();
            services.AddMvc()
                .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_1)
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>());

            services.AddAuthorization();

            services.AddTransient<Infrastructure.Notification.IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();

            services.AddHealthChecks()
                .AddDbContextCheck<IdentityDbContext>();

            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = $"{Configuration.GetValue<string>("IdentityServerAddress")}";
                    options.RequireHttpsMetadata = false;
                    options.Audience = "auth";
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseHealthChecks("/ready", new HealthCheckOptions
            {
                // The following StatusCodes are the default assignments for
                // the HealthCheckStatus properties.
                ResultStatusCodes =
                {
                    [HealthStatus.Healthy] = StatusCodes.Status200OK,
                    [HealthStatus.Degraded] = StatusCodes.Status200OK,
                    [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                },
                // The default value is false.
                AllowCachingResponses = false
            });

            app.UseMetricServer();

            if (!string.IsNullOrEmpty(Configuration["PrometheusPushGateway"]))
            {
                var metricServer = new MetricPusher(
                    endpoint: Configuration["PrometheusPushGateway"],
                    job: $"{Dns.GetHostName()}_metric_job"
                );
                metricServer.Start();
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }


            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            app.UseExceptionHandler(
                builder =>
                {
                    builder.Run(
                        async context =>
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            context.Response.Headers.Add("Access-Control-Allow-Origin", "*");

                            var error = context.Features.Get<IExceptionHandlerFeature>();
                            if (error != null)
                            {
                                context.Response.AddApplicationError(error.Error.Message);
                                await context.Response.WriteAsync(error.Error.Message).ConfigureAwait(false);
                            }
                        });
                });

            app.UseIdentityServer();
            app.UseMvcWithDefaultRoute();
        }
    }
}
