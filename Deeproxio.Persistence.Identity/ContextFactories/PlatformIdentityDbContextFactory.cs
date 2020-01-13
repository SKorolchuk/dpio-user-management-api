using System;
using System.IO;
using Deeproxio.Persistence.Identity.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Deeproxio.Persistence.Identity
{
    public class PlatformIdentityDbContextFactory : IDesignTimeDbContextFactory<PlatformIdentityDbContext>
    {
        public PlatformIdentityDbContext CreateDbContext(string[] args)
        {
            var deploymentType =
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", EnvironmentVariableTarget.Machine);

            var currentDirectory = Directory.GetCurrentDirectory();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{deploymentType}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var configurationStoreConnection = configuration.GetConnectionString(nameof(PlatformIdentityDbContext));
            var optionsBuilder = new DbContextOptionsBuilder<PlatformIdentityDbContext>();
            optionsBuilder.UseNpgsql(
                configurationStoreConnection,
                    b => b.MigrationsAssembly("Deeproxio.Persistence.Identity"));

            return new PlatformIdentityDbContext(optionsBuilder.Options);
        }
    }
}
