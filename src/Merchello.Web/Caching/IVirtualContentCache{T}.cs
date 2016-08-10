namespace Merchello.Web.Caching
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Models.EntityBase;
    using Merchello.Web.Models;

    using Umbraco.Core.Events;
    using Umbraco.Core.Models;
    using Umbraco.Core.Persistence;

    /// <summary>
    /// The VirtualContentCache interface.
    /// </summary>
    /// <typeparam name="TContent">
    /// The type of the virtual content
    /// </typeparam>
    /// <typeparam name="TEntity">
    /// The type of the entity
    /// </typeparam>
    public interface IVirtualContentCache<TContent, TEntity>
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
        /// Maps a page of keys to a PagedCollection{TContent}.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <returns>
        /// The <see cref="PagedCollection"/>.
        /// </returns>
        PagedCollection<TContent> MapPagedCollection(Page<Guid> page, string sortBy);

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

        /// <summary>
        /// Clears the runtime cache of IPublishedContent.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        void ClearVirtualCache(Guid key);
    }
}