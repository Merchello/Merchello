namespace Merchello.Core.EntityCollections
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Persistence.Querying;

    using Umbraco.Core.Persistence;

    /// <summary>
    /// The cached queryable entity collection provider base.
    /// </summary>
    /// <typeparam name="T">
    /// The type of entity
    /// </typeparam>
    public abstract class CachedQueryableEntityCollectionProviderBase<T> : EntityCollectionProviderBase<T>, ICachedQueryableEntityCollectionProvider
        where T : class, IEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CachedQueryableEntityCollectionProviderBase{T}"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        protected CachedQueryableEntityCollectionProviderBase(IMerchelloContext merchelloContext, Guid collectionKey)
            : base(merchelloContext, collectionKey)
        {
        }

        /// <summary>
        /// The get paged entity keys.
        /// </summary>
        /// <param name="args">
        /// The query arguments.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        public Page<Guid> GetPagedEntityKeys(
            Dictionary<string, object> args,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Ascending)
        {
            return this.PerformGetPagedEntityKeys(args, page, itemsPerPage, sortBy, sortDirection);
        }

        /// <summary>
        /// The get paged entity keys.
        /// </summary>
        /// <param name="args">
        /// The query arguments
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        protected abstract Page<Guid> PerformGetPagedEntityKeys(
            Dictionary<string, object> args,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Ascending);
    }
}