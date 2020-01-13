using System;
using System.IO;
using System.Linq;
using Deeproxio.Persistence.Identity.Migrator.Managers;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace Deeproxio.Persistence.Identity
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("System", LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
                    .Enrich.FromLogContext()
                    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Literate)
                    .CreateLogger();

                var deploymentType =
                    Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", EnvironmentVariableTarget.Machine);

                var currentDirectory = Directory.GetCurrentDirectory();

                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .AddJsonFile($"appsettings.{deploymentType}.json", optional: true)
                    .AddEnvironmentVariables()
                    .Build();


                Console.WriteLine("Wait...");

                var migrationManagers = new IMigrationManager[]
                {
                 new ConfigurationStoreDbContextMigrationManager(),
                 new PersistedGrantStoreDbContextMigrationManager(),
                 new PlatformIdentityDbContextMigrationManager()
                };

                foreach (var migrationManager in migrationManagers)
                {
                    migrationManager.Migrate(args, args.Any(arg => arg.ToLowerInvariant().Equals("seed")) || configuration.GetValue<bool>("Config:SeedData"));
                }

            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
