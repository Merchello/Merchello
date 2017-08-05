namespace Merchello.Core.Data.Mappings
{
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public abstract class DbEntityConfiguration<TEntity> where TEntity : class
    {
        public abstract void Configure(EntityTypeBuilder<TEntity> entity);
    }
}
