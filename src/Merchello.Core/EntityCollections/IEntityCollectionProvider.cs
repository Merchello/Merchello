namespace Merchello.Core.EntityCollections
{
    using System.Collections;
    using System.Collections.Generic;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Represents an EntityCollectionProvider.
    /// </summary>
    public interface IEntityCollectionProvider
    {
        /// <summary>
        /// The get entities.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{Object}"/>.
        /// </returns>
        IEnumerable<object> GetEntities();
            
            /// <summary>
        /// The get entities.
        /// </summary>
        /// <typeparam name="T">
        /// The type of IEntity
        /// </typeparam>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        IEnumerable<T> GetEntities<T>() where T : class, IEntity;
    }
}