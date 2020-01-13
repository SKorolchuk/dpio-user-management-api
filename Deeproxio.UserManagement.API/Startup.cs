using System;
using System.Net;
using System.Text;
using AutoMapper;
using Deeproxio.UserManagement.API.Extensions;
using Deeproxio.Infrastructure.Notification;
using Deeproxio.Persistence.Identity.Context;
using Deeproxio.Persistence.Identity.Jwt;
using FluentValidation.AspNetCore;
using Kubernetes.Configuration.Extensions.Configmap;
using Kubernetes.Configuration.Extensions.Secret;
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
using Microsoft.IdentityModel.Tokens;
using Prometheus;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Deeproxio.Persistence.Identity.Models;

namespace Deeproxio.UserManagement.API
{
    public class Startup
    {
        private string secretKey;
        private SymmetricSecurityKey signingKey;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddJsonFile($"secrets/appsettings.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsProduction() && Environment.GetEnvironmentVariable("KUBE") == "true")
            {
                builder
                    .AddKubernetesConfigmap("dpioUserManagement.API=config", reloadOnChange: true)
                    .AddKubernetesSecret("dpioUserManagement.API=secret", reloadOnChange: true);
            }

            Configuration = builder.Build();

            secretKey = Configuration.GetValue("JwtIssuerOptions::SecretKey", "iNivDmHLpUA223sqsfhqGbMRdRj1PVkH");
            signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddDbContext<PlatformIdentityDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString(nameof(PlatformIdentityDbContext))));

            services.AddSingleton<IJwtFactory, JwtFactory>();

            services.TryAddTransient<IHttpContextAccessor, HttpContextAccessor>();

            // jwt wire up
            // Get options from app settings
            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));

            // Configure JwtIssuerOptions
            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            });

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

                ValidateAudience = false,
                ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,

                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            // add identity
            var builder = services.AddIdentityCore<PlatformIdentityUser>(o =>
            {
                // configure identity options
                o.Password.RequireDigit = false;
                o.Password.RequireLowercase = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequiredLength = 6;
            });
            builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), builder.Services);
            builder.AddEntityFrameworkStores<PlatformIdentityDbContext>().AddDefaultTokenProviders();

            services.AddHttpContextAccessor();
            // Identity services
            services.TryAddScoped<IUserValidator<PlatformIdentityUser>, UserValidator<PlatformIdentityUser>>();
            services.TryAddScoped<IPasswordValidator<PlatformIdentityUser>, PasswordValidator<PlatformIdentityUser>>();
            services.TryAddScoped<IPasswordHasher<PlatformIdentityUser>, PasswordHasher<PlatformIdentityUser>>();
            services.TryAddScoped<ILookupNormalizer, UpperInvariantLookupNormalizer>();
            services.TryAddScoped<IRoleValidator<IdentityRole>, RoleValidator<IdentityRole>>();

            services.TryAddScoped<IdentityErrorDescriber>();
            services.TryAddScoped<ISecurityStampValidator, SecurityStampValidator<PlatformIdentityUser>>();
            services.TryAddScoped<ITwoFactorSecurityStampValidator, TwoFactorSecurityStampValidator<PlatformIdentityUser>>();
            services.TryAddScoped<IUserClaimsPrincipalFactory<PlatformIdentityUser>, UserClaimsPrincipalFactory<PlatformIdentityUser, IdentityRole>>();
            services.TryAddScoped<UserManager<PlatformIdentityUser>>();
            services.TryAddScoped<SignInManager<PlatformIdentityUser>>();
            services.TryAddScoped<RoleManager<IdentityRole>>();


            services.AddIdentityServer(options =>
            {
                options.Authentication.CookieAuthenticationScheme = JwtBearerDefaults.AuthenticationScheme;
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            })
            .AddOperationalStore<PersistedGrantStoreDbContext>(options =>
            {
                options.ConfigureDbContext = builder => builder.UseNpgsql(Configuration.GetConnectionString(nameof(PersistedGrantStoreDbContext)));
                options.EnableTokenCleanup = true;
                options.DefaultSchema = "id4_persist_grant";
            })
            .AddConfigurationStore<ConfigurationStoreDbContext>(options =>
            {
                options.ConfigureDbContext = builder => builder.UseNpgsql(Configuration.GetConnectionString(nameof(ConfigurationStoreDbContext)));
                options.DefaultSchema = "id4_config";
            })
            .AddAspNetIdentity<PlatformIdentityUser>()
            .AddDeveloperSigningCredential();

            services
                .AddAutoMapper(typeof(Startup).Assembly)
                .AddCors()
                .AddMvc()
                .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_3_0)
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>());

            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(configureOptions =>
                {
                    configureOptions.ClaimsIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                    configureOptions.TokenValidationParameters = tokenValidationParameters;
                    configureOptions.SaveToken = true;
                });

            // api user claim policy
            services.AddAuthorization(options =>
            {
                options.AddPolicy(nameof(PlatformIdentityUser), policy => policy.RequireClaim(Constants.Strings.JwtClaimIdentifiers.Rol, Constants.Strings.JwtClaims.ApiAccess));
            });

            services.AddHealthChecks()
                .AddDbContextCheck<PlatformIdentityDbContext>(nameof(PlatformIdentityDbContext), HealthStatus.Unhealthy)
                .AddDbContextCheck<PersistedGrantStoreDbContext>(nameof(PersistedGrantStoreDbContext), HealthStatus.Unhealthy)
                .AddDbContextCheck<ConfigurationStoreDbContext>(nameof(ConfigurationStoreDbContext), HealthStatus.Unhealthy);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseRouting();

            app.UseMetricServer();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }


            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

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
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/ready", new HealthCheckOptions()
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
                }).WithDisplayName("healthz");
            });
        }
    }
}
