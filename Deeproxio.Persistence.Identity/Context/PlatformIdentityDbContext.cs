using System;
using System.Linq;
using Deeproxio.Persistence.Configuration.Mappings;
using Deeproxio.Persistence.Identity.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Deeproxio.Persistence.Identity.Context
{
    public class PlatformIdentityDbContext : IdentityDbContext<PlatformIdentityUser>
    {
        public PlatformIdentityDbContext(DbContextOptions<PlatformIdentityDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema("identity");
            builder.UseSerialColumns();

            var mapTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                .Where(x => x.BaseType != null && x.BaseType.IsGenericType && x.BaseType.GetGenericTypeDefinition() == typeof(BaseMap<>) && x.IsSealed)
                .ToList();

            foreach (var type in mapTypes)
            {
                dynamic instance = Activator.CreateInstance(type);

                instance.Map(builder);
            }
        }
    }
}
