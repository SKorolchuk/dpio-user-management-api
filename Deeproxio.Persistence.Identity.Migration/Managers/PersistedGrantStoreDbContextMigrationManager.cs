using Deeproxio.Persistence.Identity.ContextFactories;
using Deeproxio.Persistence.Identity.Migration.Seeds;
using Microsoft.EntityFrameworkCore;

namespace Deeproxio.Persistence.Identity.Migration.Managers
{
    public class PersistedGrantStoreDbContextMigrationManager : IMigrationManager
    {
        public void Migrate(string[] args, bool withDataSeed = false)
        {
            var dbContextFactory = new PersistedGrantStoreDbContextFactory();

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
