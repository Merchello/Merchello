namespace Merchello.Core.Services
{
    using System;

    using Merchello.Core.Persistence.Querying;

    using Models.EntityBase;

    using Umbraco.Core.Persistence;
    using Umbraco.Core.Services;

    /// <summary>
    /// Marks paged cache services.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The type of entity
    /// </typeparam>
    public interface IPageCachedService<TEntity> : IService
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
        TEntity GetByKey(Guid key);


        /// <summary>
        /// Gets a <see cref="Page{TEntity}"/>
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
        /// The <see cref="Page{TEntity}"/>.
        /// </returns>
        Page<TEntity> GetPage(long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Descending);

        /////// <summary>
        /////// Performs a paged query
        /////// </summary>
        /////// <param name="page">
        /////// The page.
        /////// </param>
        /////// <param name="itemsPerPage">
        /////// The items per page.
        /////// </param>
        /////// <param name="sortBy">
        /////// The sort by.
        /////// </param>
        /////// <param name="sortDirection">
        /////// The sort direction.
        /////// </param>
        /////// <returns>
        /////// The <see cref="Page{Guid}"/>.
        /////// </returns>
        ////Page<Guid> GetPage(
        ////    long page,
        ////    long itemsPerPage,
        ////    string sortBy = "",
        ////    SortDirection sortDirection = SortDirection.Descending);
    }
}