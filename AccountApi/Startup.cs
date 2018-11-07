using System;
using System.Net;
using System.Text;
using AutoMapper;
using Deeproxio.AccountApi.Extensions;
using Deeproxio.Infrastructure.Notification;
using Deeproxio.Infrastructure.Runtime;
using Deeproxio.Persistence.Identity.Context;
using Deeproxio.Persistence.Identity.Identity;
using Deeproxio.Persistence.Identity.Jwt;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;

namespace Deeproxio.AccountApi
{
    public class Startup : IServerStartup
    {
	    private string secretKey;
	    private SymmetricSecurityKey signingKey;

		public Startup(IHostingEnvironment env)
        {
	        var builder = new ConfigurationBuilder()
		        .SetBasePath(env.ContentRootPath)
		        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
		        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

	        builder.AddEnvironmentVariables();
	        Configuration = builder.Build();

	        secretKey = Configuration.GetValue("SecretKey", "iNivDmHLpUA223sqsfhqGbMRdRj1PVkH");
			signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));
		}

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
			services.AddDbContext<IdentityDbContext>(options =>
		        options.UseNpgsql(Configuration.GetConnectionString(nameof(IdentityDbContext)),
			        b => b.MigrationsAssembly(typeof(IdentityDbContext).Assembly.FullName)));

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
				ValidateIssuer = true,
				ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

				ValidateAudience = true,
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
				.AddJwtBearer(configureOptions =>
				{
					configureOptions.ClaimsIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
					configureOptions.TokenValidationParameters = tokenValidationParameters;
					configureOptions.SaveToken = true;
				});

			// api user claim policy
			services.AddAuthorization(options =>
			{
				options.AddPolicy(nameof(ApplicationUser), policy => policy.RequireClaim(Constants.Strings.JwtClaimIdentifiers.Rol, Constants.Strings.JwtClaims.ApiAccess));
			});

			// add identity
			var builder = services.AddIdentityCore<ApplicationUser>(o =>
			{
				// configure identity options
				o.Password.RequireDigit = false;
				o.Password.RequireLowercase = false;
				o.Password.RequireUppercase = false;
				o.Password.RequireNonAlphanumeric = false;
				o.Password.RequiredLength = 6;
			});
			builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), builder.Services);
			builder.AddEntityFrameworkStores<IdentityDbContext>().AddDefaultTokenProviders();

			services.AddAutoMapper();
			services.AddMvc().AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>());

			services.AddTransient<Infrastructure.Notification.IEmailSender, AuthMessageSender>();
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


			app.UseAuthentication();
			app.UseDefaultFiles();
			app.UseStaticFiles();
	        app.UseMvc();
		}
    }
}
