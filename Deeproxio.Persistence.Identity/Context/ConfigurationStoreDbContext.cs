using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Deeproxio.Persistence.Identity.Context
{
    public class ConfigurationStoreDbContext : ConfigurationDbContext<ConfigurationStoreDbContext>
    {
        public ConfigurationStoreDbContext(
            DbContextOptions<ConfigurationStoreDbContext> options) 
            : base(options,
                  new IdentityServer4.EntityFramework.Options.ConfigurationStoreOptions
                    {
                         DefaultSchema = "id4_config"
                    }
                  )
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.UseSerialColumns();
        }
    }
}
