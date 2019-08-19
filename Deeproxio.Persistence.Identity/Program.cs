using System;
using System.IO;
using System.Linq;
using Deeproxio.Persistence.Identity.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Deeproxio.Persistence.Identity
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Wait...");

            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            IConfigurationRoot configuration = builder.Build();

            var connectionString = configuration.GetConnectionString(nameof(IdentityDbContext));

            Console.WriteLine($"Using DB connection: {connectionString}");
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<IdentityDbContext>();
            dbContextOptionsBuilder.UseNpgsql(connectionString);
            using (var context = new IdentityDbContext(dbContextOptionsBuilder.Options))
            {
                Console.WriteLine($"Current users: {string.Concat(context.Users.Select(it => it.UserName).ToArray().Select(x => x + " "))}");
            }

            Console.ReadKey();
        }
    }
}
