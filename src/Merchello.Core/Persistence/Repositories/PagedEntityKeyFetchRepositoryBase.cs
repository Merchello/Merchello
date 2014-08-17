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
    internal abstract class PagedEntityKeyFetchRepositoryBase<TEntity, TDto> : MerchelloPetaPocoRepositoryBase<TEntity>, IPagedEntityKeyFetchRepository<TEntity, TDto> 
        where TEntity : class, IEntity
        where TDto : class, IPageableDto
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PagedEntityKeyFetchRepositoryBase{TEntity,TDto}"/> class. 
        /// </summary>
        /// <param name="work">
        /// The work.
        /// </param>
        /// <param name="cache">
        /// The cache.
        /// </param>
        protected PagedEntityKeyFetchRepositoryBase(IDatabaseUnitOfWork work, IRuntimeCacheProvider cache) 
            : base(work, cache)
        {
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
        public Page<Guid> GetPagedKeys(long page, long itemsPerPage, IQuery<TEntity> query, string orderExpression, SortDirection sortDirection = SortDirection.Descending)
        {
            var sqlClause = new Sql();
            sqlClause.Select("*").From<TDto>();

            var translator = new SqlTranslator<TEntity>(sqlClause, query);
            var sql = translator.Translate();

            return GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);
        }

        public abstract Page<Guid> Search(string searchTerm, long page, long itemsPerPage, string orderExpression, SortDirection sortDirection = SortDirection.Descending);

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
        protected Page<Guid> GetPagedKeys(long page, long itemsPerPage, Sql sql, string orderExpression, SortDirection sortDirection = SortDirection.Descending)
        {
            if (!string.IsNullOrEmpty(orderExpression))
            {
                sql.Append(sortDirection == SortDirection.Ascending
                    ? string.Format("ORDER BY {0} ASC", orderExpression)
                    : string.Format("ORDER BY {0} DESC", orderExpression));
            }

            var p = Database.Page<TDto>(page, itemsPerPage, sql);

            return new Page<Guid>()
            {
                CurrentPage = p.CurrentPage,
                ItemsPerPage = p.ItemsPerPage,
                TotalItems = p.TotalItems,
                TotalPages = p.TotalPages,
                Items = p.Items.Select(x => x.Key).ToList()
            };
        }
    }
}