namespace Merchello.Core.EntityCollections
{
    using System;

    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Persistence.Querying;

    using Umbraco.Core.Persistence;

    /// <summary>
    /// The cached entity collection provider base.
    /// </summary>
    /// <typeparam name="T">
    /// The type of entity
    /// </typeparam>
    public abstract class CachedEntityCollectionProviderBase<T> : EntityCollectionProviderBase<T>, ICachedEntityCollectionProvider
        where T : class, IEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CachedEntityCollectionProviderBase{T}"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        protected CachedEntityCollectionProviderBase(IMerchelloContext merchelloContext, Guid collectionKey)
            : base(merchelloContext, collectionKey)
        {
        }

        /// <summary>
        /// The get paged entity keys.
        /// </summary>
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
        /// <remarks>
        /// All we're doing here is keeping a bit of control in case we need to do some processing before or after
        /// later on.
        /// </remarks>
        public Page<Guid> GetPagedEntityKeys(long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Ascending)
        {
            return this.PerformGetPagedEntityKeys(page, itemsPerPage, sortBy, sortDirection);
        }

        /// <summary>
        /// The get paged entity keys.
        /// </summary>
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
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Ascending);
    }
}