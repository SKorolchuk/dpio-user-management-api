using Deeproxio.Domain.Models;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Deeproxio.Persistence.Configuration.Mappings
{
    public abstract class BaseMap<T> where T: BaseEntity
    {
        protected Action<EntityTypeBuilder<T>> BuilderFunc;

        protected BaseMap()
        {
        }

        public virtual void Map(ModelBuilder builder)
        {
            builder.Entity<T>(BuilderFunc);
        }
    }
}
