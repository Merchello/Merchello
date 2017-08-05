namespace Merchello.Core.Data.Mappings
{
    using System;

    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public abstract class DbEntityConfiguration<TEntity> where TEntity : class
    {
        public virtual Action<EntityTypeBuilder<TEntity>> Build()
        {
            return this.Configure;
        }

        public abstract void Configure(EntityTypeBuilder<TEntity> entity);
    }
}
