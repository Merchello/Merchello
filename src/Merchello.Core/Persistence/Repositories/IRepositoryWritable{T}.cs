namespace Merchello.Core.Persistence.Repositories
{
    /// <summary>
    /// Defines the implementation of a writable repository
    /// </summary>
    /// <typeparam name="TEntity">
    /// The type of entity
    /// </typeparam>
    public interface IRepositoryWritable<in TEntity>
    {
        /// <summary>
        /// Adds or Updates an Entity
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        void AddOrUpdate(TEntity entity);

        /// <summary>
        /// Deletes an Entity
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        void Delete(TEntity entity);
    }
}