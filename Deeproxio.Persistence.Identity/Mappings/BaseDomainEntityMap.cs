using Deeproxio.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Deeproxio.Persistence.Configuration.Mappings
{
    public abstract class BaseDomainEntityMap<T> : BaseMap<T> where T : BaseDomainEntity
    {
        protected BaseDomainEntityMap()
        {
            BuilderFunc = entity =>
            {
                entity.ToTable($"{GetTableName()}");

                entity.HasKey(e => e.Id).HasName($"{GetTableName()}_id_primary");

                entity.HasIndex(e => e.Id)
                    .HasName($"{GetTableName()}_id_uindex")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName($"{GetTableName()}_id")
                    .ValueGeneratedOnAdd()
                    .HasDefaultValue($"nextval('\"{GetTableName()}_id_seq\"')");

                entity.Property(e => e.CreatedOn).HasColumnName("created_on");

                entity.Property(e => e.ModifiedOn).HasColumnName("modified_on");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");

                BuildDomainSpecificMapping(entity);
            };
        }

        protected abstract void BuildDomainSpecificMapping(EntityTypeBuilder<T> entity);

        public override void Map(ModelBuilder builder)
        {
            base.Map(builder);

            builder.HasSequence<int>($"{GetTableName()}_id_seq")
                .StartsAt(1)
                .IncrementsBy(1);
        }

        protected string GetTableName()
        {
            return $"{nameof(T).ToLowerInvariant()}";
        }
    }
}
