namespace Merchello.Core
{
    using Merchello.Core.Data.Mappings;

    using Microsoft.EntityFrameworkCore;

    public static partial class Extensions
    {
        internal static void AddConfiguration<TEntity>(
            this ModelBuilder modelBuilder,
            DbEntityConfiguration<TEntity> entityConfiguration) where TEntity : class
        {
            modelBuilder.Entity<TEntity>(entityConfiguration.Configure);
        }
    }
}