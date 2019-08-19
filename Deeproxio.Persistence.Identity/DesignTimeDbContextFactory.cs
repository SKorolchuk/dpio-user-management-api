using System;
using System.IO;
using Deeproxio.Persistence.Identity.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Deeproxio.Persistence.Identity
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
    {
        public IdentityDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            var pgConnectionString = $"Server={configuration["Postgres:Host"]};Port={configuration["Postgres:Port"]};User Id={configuration["Postgres:Username"]};Password={configuration["Postgres:Password"]};";

            var pgConnection = new NpgsqlConnection(pgConnectionString);
            try
            {
                var createDbCmd =
                    new NpgsqlCommand(
                        $"CREATE DATABASE {configuration["Postgres:Database"]} WITH OWNER {configuration["Postgres:Username"]} ENCODING 'UTF8';",
                        pgConnection);

                pgConnection.Open();
                createDbCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database already exists {ex.Message}");
            }
            finally
            {
                pgConnection.Close();
            }


            var builder = new DbContextOptionsBuilder<IdentityDbContext>();
            var connectionString = configuration.GetConnectionString(nameof(IdentityDbContext));
            builder.UseNpgsql(connectionString);
            return new IdentityDbContext(builder.Options);
        }
    }
}