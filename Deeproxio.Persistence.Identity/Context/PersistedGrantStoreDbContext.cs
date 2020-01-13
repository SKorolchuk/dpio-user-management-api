using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Deeproxio.Persistence.Identity.Context
{
    public class PersistedGrantStoreDbContext : PersistedGrantDbContext<PersistedGrantStoreDbContext>
    {
        public PersistedGrantStoreDbContext(
            DbContextOptions<PersistedGrantStoreDbContext> options)
            : base(options,
                      new IdentityServer4.EntityFramework.Options.OperationalStoreOptions
                      {
                          DefaultSchema = "id4_persist_grant"
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
