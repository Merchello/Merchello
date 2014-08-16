namespace Merchello.Core.Services
{
    using System;
    using System.Web.UI;

    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Persistence.Querying;

    using Umbraco.Core.Persistence;

    /// <summary>
    /// The page cached service base.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The type of entity
    /// </typeparam>
    public abstract class PageCachedServiceBase<TEntity> : IPageCachedService<TEntity>
        where TEntity : class, IEntity
    {
        /// <summary>
        /// Gets an entity by it's unique key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="TEntity"/>.
        /// </returns>
        public abstract TEntity GetByKey(Guid key);

        /// <summary>
        /// Performs a paged query
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
        internal abstract Page<Guid> GetPage(
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending);
    }
}