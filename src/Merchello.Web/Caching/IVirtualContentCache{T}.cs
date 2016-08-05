namespace Merchello.Web.Caching
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Models.EntityBase;

    using Umbraco.Core.Events;
    using Umbraco.Core.Models;

    /// <summary>
    /// The VirtualContentCache interface.
    /// </summary>
    /// <typeparam name="TContent">
    /// The type of the virtual content
    /// </typeparam>
    /// <typeparam name="TEntity">
    /// The type of the entity
    /// </typeparam>
    public interface IVirtualContentCache<out TContent, TEntity>
        where TContent : IPublishedContent
        where TEntity : IEntity

    {
        /// <summary>
        /// Gets the virtual content by it's unique key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The virtual <see cref="IPublishedContent"/>.
        /// </returns>
        TContent GetByKey(Guid key);

        /// <summary>
        /// Gets virtual <see cref="IPublishedContent"/> by it's key.
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{TContent}"/>.
        /// </returns>
        IEnumerable<TContent> GetByKeys(IEnumerable<Guid> keys);


        /// <summary>
        /// Clears the runtime cache of IPublishedContent.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        void ClearVirtualCache(SaveEventArgs<TEntity> e);

        /// <summary>
        /// Clears the runtime cache of IPublishedContent.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        void ClearVirtualCache(DeleteEventArgs<TEntity> e);
    }
}