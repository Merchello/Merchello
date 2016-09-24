namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Represents a read repository.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The type of the entity
    /// </typeparam>
    internal interface IRepositoryReadable<out TEntity> : IRepository
        where TEntity : class, IEntity
    {
        /// <summary>
        /// Gets an Entity it's key
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="TEntity"/>.
        /// </returns>
        TEntity Get(Guid key);

        /// <summary>
        /// Gets all entities of the specified type
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The collection of entities
        /// </returns>
        IEnumerable<TEntity> GetAll(params Guid[] keys);

        /// <summary>
        /// Gets a value indicating whether an Entity with the specified key exists.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The value indicating whether an Entity with the specified key exists.
        /// </returns>
        bool Exists(Guid key);
    }
}