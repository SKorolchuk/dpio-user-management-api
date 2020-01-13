using System;
using System.IO;
using Deeproxio.Persistence.Identity.Context;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Deeproxio.Persistence.Identity.ContextFactories
{
    public class ConfigurationStoreContextFactory : IDesignTimeDbContextFactory<ConfigurationStoreDbContext>
    {
        public ConfigurationStoreDbContext CreateDbContext(string[] args)
        {
            var deploymentType =
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var currentDirectory = Directory.GetCurrentDirectory();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{deploymentType}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var configurationStoreConnection = configuration.GetConnectionString(nameof(ConfigurationStoreDbContext));
            var optionsBuilder = new DbContextOptionsBuilder<ConfigurationStoreDbContext>();
            optionsBuilder.UseNpgsql(
                configurationStoreConnection,
                    b => b.MigrationsAssembly("Deeproxio.Persistence.Identity"));

            return new ConfigurationStoreDbContext(optionsBuilder.Options);
        }
    }
}
