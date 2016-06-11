namespace Merchello.Core.EntityCollections
{
    using System;

    using Merchello.Core.Persistence.Querying;

    using Umbraco.Core.Persistence;

    /// <summary>
    /// Defines a <see cref="IEntityCollectionProvider"/> that has cached Examine values.
    /// </summary>
    public interface ICachedEntityCollectionProvider : IEntityCollectionProvider
    {
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
        /// The <see cref="Page{T}"/>.
        /// </returns>
        Page<Guid> GetPagedEntityKeys(long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Ascending);
    }
}