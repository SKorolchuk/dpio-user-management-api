using Deeproxio.Persistence.Identity.Migrator.Seeds;
using Microsoft.EntityFrameworkCore;

namespace Deeproxio.Persistence.Identity.Migrator.Managers
{
    public class PlatformIdentityDbContextMigrationManager : IMigrationManager
    {
        public void Migrate(string[] args, bool withDataSeed = false)
        {
            var dbContextFactory = new PlatformIdentityDbContextFactory();

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
