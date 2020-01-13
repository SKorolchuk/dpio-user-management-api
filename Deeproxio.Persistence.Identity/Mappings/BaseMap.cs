using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Deeproxio.Persistence.Identity.Models;

namespace Deeproxio.Persistence.Configuration.Mappings
{
    public abstract class BaseMap<T> where T : class, IEntity
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
