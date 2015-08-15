namespace Merchello.Core.EntityCollections
{
    using System.Collections;
    using System.Collections.Generic;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Defines a Queryable Entity Collection Provider.
    /// </summary>
    public interface IQueryableEntityCollectionProvider : IEntityCollectionProvider
    {
        /// <summary>
        /// The get entities.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        IEnumerable<object> GetEntities(Dictionary<string, object> args);

        /// <summary>
        /// The get entities.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <typeparam name="T">
        /// The type of entity
        /// </typeparam>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        IEnumerable<T> GetEntities<T>(Dictionary<string, object> args) where T : class, IEntity;
    }
}