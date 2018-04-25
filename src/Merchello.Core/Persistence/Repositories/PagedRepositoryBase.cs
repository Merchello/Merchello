namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Models.EntityBase;
    using Models.Rdbms;
    using Querying;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Querying;
    using Umbraco.Core.Persistence.SqlSyntax;

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
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="sqlSyntax">
        /// The SQL Syntax.
        /// </param>
        protected PagedRepositoryBase(IDatabaseUnitOfWork work, ILogger logger, ISqlSyntaxProvider sqlSyntax) 
            : base(work, logger, sqlSyntax)
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
            sqlClause.Select("*").From<TDto>(SqlSyntax);

            var translator = new SqlTranslator<TEntity>(sqlClause, query);
            var sql = translator.Translate();

            var p = GetDtoPage(page, itemsPerPage, sql, orderExpression, sortDirection);

            return this.MapPageDtoToPageEntity(p);
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
            sqlClause.Select("*").From<TDto>(SqlSyntax);

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
        /// Gets the cache key for Request caching paged collections.
        /// </summary>
        /// <param name="methodName">
        /// The method name.
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
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected static string GetPagedDtoCacheKey(string methodName,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending,
            IDictionary<string, string> args = null)
        {
            return Core.Cache.CacheKeys.GetPagedKeysCacheKey<TDto>(methodName, page, itemsPerPage, orderExpression, sortDirection, args);
        }

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
        /// The SQL.
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
        protected Page<TDto> GetDtoPage(long page, long itemsPerPage, Sql sql, string orderExpression, SortDirection sortDirection = SortDirection.Descending)
        {
            if (!string.IsNullOrEmpty(orderExpression))
            {
                sql.Append(sortDirection == SortDirection.Ascending
                    ? string.Format("ORDER BY {0} ASC", orderExpression)
                    : string.Format("ORDER BY {0} DESC", orderExpression));
            }

            return Database.Page<TDto>(page, itemsPerPage, sql);
        }

        /// <summary>
        /// The map page dto to page entity.
        /// </summary>
        /// <param name="dtoPage">
        /// The <see cref="Page{TDto}"/>.
        /// </param>
        /// <returns>
        /// The <see cref="Page{TEntity}"/>.
        /// </returns>
        protected Page<TEntity> MapPageDtoToPageEntity(Page<TDto> dtoPage)
        {
            return new Page<TEntity>()
            {
                CurrentPage = dtoPage.CurrentPage,
                ItemsPerPage = dtoPage.ItemsPerPage,
                TotalItems = dtoPage.TotalItems,
                TotalPages = dtoPage.TotalPages,
                Items = dtoPage.Items.Select(x => Get(x.Key)).ToList()
            };
        }

    }
}