using Deeproxio.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Deeproxio.Persistence.Configuration.Mappings
{
    public sealed class NewsMap : BaseMap<News>
    {
        public NewsMap()
        {
            BuilderFunc = entity =>
            {
                entity.ToTable("news");

                entity.HasIndex(e => e.Id)
                    .HasName("news_id_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.Name)
                    .HasName("news_name_uindex")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateTS).HasColumnName("createts");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Name).HasColumnName("name");

                entity.Property(e => e.UpdateTS).HasColumnName("updatets");
            };
        }

        public override void Map(ModelBuilder builder)
        {
            base.Map(builder);

            builder.HasSequence("news_id_seq")
                .HasMin(1)
                .HasMax(2147483647);
        }
    }
}
