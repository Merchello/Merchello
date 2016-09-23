namespace Merchello.Core.Persistence.Repositories
{
    /// <summary>
    /// Defines the implementation of a writable repository
    /// </summary>
    public interface IWriteRepository<in TEntity>
    {
        /// <summary>
        /// Adds or Updates an Entity
        /// </summary>
        void AddOrUpdate(TEntity entity);

        /// <summary>
        /// Deletes an Entity
        /// </summary>
        void Delete(TEntity entity);
    }
}