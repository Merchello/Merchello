namespace Merchello.Core.EntityCollections
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Persistence.Querying;

    using Umbraco.Core.Persistence;

    /// <summary>
    /// Defines a QueryableCachedEntityCollectionProvider.
    /// </summary>
    public interface ICachedQueryableEntityCollectionProvider
    {
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
        /// The <see cref="Page{T}"/>.
        /// </returns>
        Page<Guid> GetPagedEntityKeys(Dictionary<string, object> args, long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Ascending); 
    }
}