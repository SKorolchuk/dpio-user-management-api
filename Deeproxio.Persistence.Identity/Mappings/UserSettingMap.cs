using Deeproxio.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Deeproxio.Persistence.Configuration.Mappings
{
    public sealed class UserSettingMap : BaseDomainEntityMap<UserSetting>
    {
        public UserSettingMap() : base()
        {
        }

        public override void Map(ModelBuilder builder)
        {
            base.Map(builder);
        }

        protected override void BuildDomainSpecificMapping(EntityTypeBuilder<UserSetting> entity)
        {
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Value).HasColumnName("value");
            entity.Property(e => e.Type).HasColumnName("type");

            entity.HasOne(e => e.SettingFor)
                    .WithMany(e => e.Settings)
                    .IsRequired()
                    .HasConstraintName($"FK_{GetTableName()}_user_setting_user_idenity")
                    .HasForeignKey("user_id")
                    .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
