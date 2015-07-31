namespace Merchello.Core.EntityCollections
{
    using System.Collections.Generic;
    using System.Web.UI;

    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Persistence.Querying;

    using Umbraco.Core.Persistence;

    /// <summary>
    /// The EntityCollectionProvider interface.
    /// </summary>
    /// <typeparam name="T">
    /// The type of entity
    /// </typeparam>
    public interface IEntityCollectionProvider<T>
        where T : IEntity
    {
        /// <summary>
        /// The get entities.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        IEnumerable<T> GetEntities();

        /// <summary>
        /// The get entities.
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
        /// The <see cref="Page"/>.
        /// </returns>
        Page<T> GetPagedEntities(long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Ascending);        
    }
}