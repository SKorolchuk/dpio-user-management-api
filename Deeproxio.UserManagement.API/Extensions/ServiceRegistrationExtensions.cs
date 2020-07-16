using System;
using System.Text;
using Deeproxio.Persistence.Identity.Context;
using Deeproxio.Persistence.Identity.Jwt;
using Deeproxio.Persistence.Identity.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;

namespace Deeproxio.UserManagement.API.Extensions
{
    public static class ServiceRegistrationExtensions
    {
        public static void AddIndentityServerDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            string secretKey = configuration.GetValue("JwtIssuerOptions:SecretKey", "");
            if (string.IsNullOrWhiteSpace(secretKey))
            {
                throw new ArgumentNullException(secretKey);
            }

            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));

            services.AddSingleton<IJwtFactory, JwtFactory>();
            services.TryAddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddHttpContextAccessor();

            services.AddDbContext<PlatformIdentityDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString(nameof(PlatformIdentityDbContext))));

            var jwtAppSettingOptions = configuration.GetSection(nameof(JwtIssuerOptions));

            // Configure JwtIssuerOptions
            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            });           

            // add identity
            var builder = services.AddIdentityCore<PlatformIdentityUser>(o =>
            {
                // configure identity options
                o.Password.RequireDigit = true;
                o.Password.RequireLowercase = true;
                o.Password.RequireUppercase = true;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequiredLength = 8;
            });

            builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), builder.Services);
            builder.AddEntityFrameworkStores<PlatformIdentityDbContext>().AddDefaultTokenProviders();

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
                options.ConfigureDbContext = builder => builder.UseNpgsql(configuration.GetConnectionString(nameof(PersistedGrantStoreDbContext)));
                options.EnableTokenCleanup = true;
                options.DefaultSchema = "id4_persist_grant";
            })
            .AddConfigurationStore<ConfigurationStoreDbContext>(options =>
            {
                options.ConfigureDbContext = builder => builder.UseNpgsql(configuration.GetConnectionString(nameof(ConfigurationStoreDbContext)));
                options.DefaultSchema = "id4_config";
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

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.ClaimsIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                    options.TokenValidationParameters = tokenValidationParameters;
                    options.SaveToken = true;
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(nameof(PlatformIdentityUser), policy => policy.RequireClaim(Constants.Strings.JwtClaimIdentifiers.Rol, Constants.Strings.JwtClaims.ApiAccess));
            });
        }
    }
}
