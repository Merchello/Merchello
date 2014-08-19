namespace Merchello.Core.Services
{
    using System;

    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Models.Rdbms;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Core.Persistence.Repositories;

    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Querying; 

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
        public abstract Page<TEntity> GetPage(long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Descending);
    
        /// <summary>
        /// The count.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        internal abstract int Count(IQuery<TEntity> query);

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
        internal abstract Page<Guid> GetPagedKeys(
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending);

        /// <summary>
        /// Gets a page.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="query">
        /// The query.
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
        /// <typeparam name="TDto">
        /// The type of dto
        /// </typeparam>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        protected virtual Page<Guid> GetPagedKeys<TDto>(
            IPagedRepository<TEntity, TDto> repository,
            IQuery<TEntity> query,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending) where TDto : IPageableDto
        {
            using (repository)
            {
                return repository.GetPagedKeys(page, itemsPerPage, query, ValidateSortByField(sortBy), sortDirection);
            }
        }

        /// <summary>
        /// Performs a paged search based on a term
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="searchTerm">
        /// The search term.
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
        /// <typeparam name="TDto">
        /// The type of <see cref="TDto"/>
        /// </typeparam>
        /// <returns>
        /// The <see cref="Page{TDto}"/>.
        /// </returns>
        protected virtual Page<Guid> Search<TDto>(
            IPagedRepository<TEntity, TDto> repository, 
            string searchTerm,
            long page,
            long itemsPerPage,
            string sortBy,
            SortDirection sortDirection = SortDirection.Descending)
            where TDto : IPageableDto
        {
            using (repository)
            {
                return repository.SearchKeys(searchTerm, page, itemsPerPage, sortBy, sortDirection);
            }
        }

        /// <summary>
        /// Validates the sort by string is a valid sort by field
        /// </summary>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <returns>
        /// A validated database field name.
        /// </returns>
        protected abstract string ValidateSortByField(string sortBy);
    }
}