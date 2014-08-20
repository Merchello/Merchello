namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using System.Linq;
    using Models.EntityBase;
    using Models.Rdbms;
    using Querying;    
    using Umbraco.Core.Cache;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Querying;
    using UnitOfWork;

    /// <summary>
    /// Defines a repository that provides paged ID queries
    /// </summary>
    /// <typeparam name="TEntity">
    /// The type of entity
    /// </typeparam>
    /// <typeparam name="TDto">
    /// The type of the DTO class
    /// </typeparam>
    internal abstract class PagedRepositoryBase<TEntity, TDto> : MerchelloPetaPocoRepositoryBase<TEntity>, IPagedRepository<TEntity, TDto> 
        where TEntity : class, IEntity
        where TDto : class, IPageableDto
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PagedRepositoryBase{TEntity,TDto}"/> class. 
        /// </summary>
        /// <param name="work">
        /// The work.
        /// </param>
        /// <param name="cache">
        /// The cache.
        /// </param>
        protected PagedRepositoryBase(IDatabaseUnitOfWork work, IRuntimeCacheProvider cache) 
            : base(work, cache)
        {
        }

        /// <summary>
        /// Gets a page of <see cref="TEntity"/>
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <param name="orderExpression">
        /// The order expression.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{TEntity}"/>.
        /// </returns>
        public virtual Page<TEntity> GetPage(long page, long itemsPerPage, IQuery<TEntity> query, string orderExpression, SortDirection sortDirection = SortDirection.Descending)
        {
            var sqlClause = new Sql();
            sqlClause.Select("*").From<TDto>();

            var translator = new SqlTranslator<TEntity>(sqlClause, query);
            var sql = translator.Translate();

            var p = GetDtoPage(page, itemsPerPage, sql, orderExpression, sortDirection);

            return new Page<TEntity>()
            {
                CurrentPage = p.CurrentPage,
                ItemsPerPage = p.ItemsPerPage,
                TotalItems = p.TotalItems,
                TotalPages = p.TotalPages,
                Items = p.Items.Select(x => Get(x.Key)).ToList()
            };
        }

        /// <summary>
        /// Get paged keys.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <param name="orderExpression">
        /// The order expression.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The page of data.
        /// </returns>
        public virtual Page<Guid> GetPagedKeys(long page, long itemsPerPage, IQuery<TEntity> query, string orderExpression, SortDirection sortDirection = SortDirection.Descending)
        {
            var sqlClause = new Sql();
            sqlClause.Select("*").From<TDto>();

            var translator = new SqlTranslator<TEntity>(sqlClause, query);
            var sql = translator.Translate();

            return GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);
        }

        /// <summary>
        /// The search.
        /// </summary>
        /// <param name="searchTerm">
        /// The search term.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="orderExpression">
        /// The order expression.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        public abstract Page<Guid> SearchKeys(string searchTerm, long page, long itemsPerPage, string orderExpression, SortDirection sortDirection = SortDirection.Descending);

        /// <summary>
        /// Get the paged keys.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sql">
        /// The <see cref="Sql"/>.
        /// </param>
        /// <param name="orderExpression">
        /// The order expression.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        protected virtual Page<Guid> GetPagedKeys(long page, long itemsPerPage, Sql sql, string orderExpression, SortDirection sortDirection = SortDirection.Descending)
        {            
            var p = GetDtoPage(page, itemsPerPage, sql, orderExpression, sortDirection);

            return new Page<Guid>()
            {
                CurrentPage = p.CurrentPage,
                ItemsPerPage = p.ItemsPerPage,
                TotalItems = p.TotalItems,
                TotalPages = p.TotalPages,
                Items = p.Items.Select(x => x.Key).ToList()
            };
        }

        /// <summary>
        /// Gets a paged <see cref="TDto"/> for the query.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sql">
        /// The sql.
        /// </param>
        /// <param name="orderExpression">
        /// The order expression.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{TDto}"/>.
        /// </returns>
        private Page<TDto> GetDtoPage(long page, long itemsPerPage, Sql sql, string orderExpression, SortDirection sortDirection = SortDirection.Descending)
        {
            if (!string.IsNullOrEmpty(orderExpression))
            {
                sql.Append(sortDirection == SortDirection.Ascending
                    ? string.Format("ORDER BY {0} ASC", orderExpression)
                    : string.Format("ORDER BY {0} DESC", orderExpression));
            }

            return Database.Page<TDto>(page, itemsPerPage, sql);
        }
    }
}