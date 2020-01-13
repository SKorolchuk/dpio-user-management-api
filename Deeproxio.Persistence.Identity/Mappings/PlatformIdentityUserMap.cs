using Deeproxio.Persistence.Configuration.Mappings;
using Deeproxio.Persistence.Identity.Models;
using Microsoft.EntityFrameworkCore;

namespace Deeproxio.Persistence.Identity.Mappings
{
    public sealed class PlatformIdentityUserMap : BaseMap<PlatformIdentityUser>
    {
        public PlatformIdentityUserMap() : base()
        {
            BuilderFunc = entity =>
            {
                entity.Property(e => e.FirstName).HasColumnName("first_name");
                entity.Property(e => e.LastName).HasColumnName("last_name");
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.Age).HasColumnName("age");
                entity.HasMany(e => e.Settings).WithOne(e => e.SettingFor);
            };
        }

        public override void Map(ModelBuilder builder)
        {
            base.Map(builder);
        }
    }
}
