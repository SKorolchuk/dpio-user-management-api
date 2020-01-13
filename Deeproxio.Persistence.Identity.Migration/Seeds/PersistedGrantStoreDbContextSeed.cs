using System;
using Deeproxio.Persistence.Identity.Context;
using Serilog;

namespace Deeproxio.Persistence.Identity.Migrator.Seeds
{
    public static class PersistedGrantStoreDbContextSeed
    {
        public static void SeedData(this PersistedGrantStoreDbContext context)
        {
            try
            {
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Error occured while saving data.");
            }
        }
    }
}
