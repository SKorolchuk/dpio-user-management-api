using System;
using System.IO;
using Deeproxio.Persistence.Identity.Context;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Deeproxio.Persistence.Identity.ContextFactories
{
    public class PersistedGrantStoreDbContextFactory : IDesignTimeDbContextFactory<PersistedGrantStoreDbContext>
    {
        public PersistedGrantStoreDbContext CreateDbContext(string[] args)
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

            var configurationStoreConnection = configuration.GetConnectionString(nameof(PersistedGrantStoreDbContext));
            var optionsBuilder = new DbContextOptionsBuilder<PersistedGrantStoreDbContext>();
            optionsBuilder.UseNpgsql(
                configurationStoreConnection,
                    b => b.MigrationsAssembly("Deeproxio.Persistence.Identity"));

            return new PersistedGrantStoreDbContext(optionsBuilder.Options);
        }
    }
}
