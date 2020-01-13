using Deeproxio.Persistence.Identity.ContextFactories;
using Deeproxio.Persistence.Identity.Migrator.Seeds;
using Microsoft.EntityFrameworkCore;

namespace Deeproxio.Persistence.Identity.Migrator.Managers
{
    public class ConfigurationStoreDbContextMigrationManager : IMigrationManager
    {
        public void Migrate(string[] args, bool withDataSeed = false)
        {
            var dbContextFactory = new ConfigurationStoreContextFactory();

            using (var context = dbContextFactory.CreateDbContext(args))
            {
                context.Database.Migrate();

                if (withDataSeed)
                {
                    context.SeedData();
                }
            }
        }
    }
}
