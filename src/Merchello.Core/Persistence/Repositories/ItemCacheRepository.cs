namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Factories;
    using Models;
    using Models.EntityBase;
    using Models.Rdbms;
    using Querying;
    using Umbraco.Core;
    using Umbraco.Core.Cache;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Querying;
    using Umbraco.Core.Persistence.SqlSyntax;

    using IDatabaseUnitOfWork = UnitOfWork.IDatabaseUnitOfWork;

    /// <summary>
    /// The item cache repository.
    /// </summary>
    internal class ItemCacheRepository : MerchelloPetaPocoRepositoryBase<IItemCache>, IItemCacheRepository
    {
        /// <summary>
        /// The <see cref="IItemCacheLineItemRepository"/>.
        /// </summary>
        private readonly IItemCacheLineItemRepository _itemCacheLineItemRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemCacheRepository"/> class.
        /// </summary>
        /// <param name="work">
        /// The work.
        /// </param>
        /// <param name="cache">
        /// The cache.
        /// </param>
        /// <param name="itemCacheLineItemRepository">
        /// The item cache line item repository.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="sqlSyntax">
        /// The SQL syntax.
        /// </param>
        public ItemCacheRepository(IDatabaseUnitOfWork work, IRuntimeCacheProvider cache, IItemCacheLineItemRepository itemCacheLineItemRepository, ILogger logger, ISqlSyntaxProvider sqlSyntax)
            : base(work, cache, logger, sqlSyntax)
        {
            _itemCacheLineItemRepository = itemCacheLineItemRepository;
        }


        /// <summary>
        /// Gets a page of <see cref="IItemCache"/>
        /// </summary>
        /// <param name="itemCacheTfKey">
        /// The item cache type.
        /// </param>
        /// <param name="startDate">
        /// The start Date.
        /// </param>
        /// <param name="endDate">
        /// The end Date.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="orderExpression">
        /// The sort by field.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{IItemCache}"/>.
        /// </returns>
        public Page<IItemCache> GetCustomerItemCachePage(
            Guid itemCacheTfKey,
            DateTime startDate,
            DateTime endDate,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var p = GetPageKeys(itemCacheTfKey, startDate, endDate, page, itemsPerPage, orderExpression, sortDirection);

            return new Page<IItemCache>()
                {
                    Items = p.Items.Select(x => this.Get(x.ItemCacheKey)).ToList(),
                    ItemsPerPage = p.ItemsPerPage,
                    TotalItems = p.TotalItems,
                    TotalPages = p.TotalPages,
                    CurrentPage = p.CurrentPage,
                };
        }

        /// <summary>
        /// Gets the count of of item caches for a customer type for a given date range.
        /// </summary>
        /// <param name="itemCacheTfKey">
        /// The item cache type field key.
        /// </param>
        /// <param name="customerType">
        /// The customer type.
        /// </param>
        /// <param name="startDate">
        /// The start Date.
        /// </param>
        /// <param name="endDate">
        /// The end Date.
        /// </param>
        /// <returns>
        /// The count of item caches.
        /// </returns>
        public int Count(Guid itemCacheTfKey, CustomerType customerType, DateTime startDate, DateTime endDate)
        {
            var table = customerType == CustomerType.Anonymous ? "[merchAnonymousCustomer]" : "[merchCustomer]";

            var querySql = @"SELECT  COUNT(*) AS cacheCount  
                FROM	merchItemCache T1
                INNER JOIN (
	                SELECT	pk
	                FROM " + table + @"
	                WHERE	lastActivityDate BETWEEN @start AND @end
                ) Q1 ON T1.entityKey = Q1.pk
                INNER JOIN (
	                SELECT	COUNT(*) AS itemCount,
			                itemCacheKey
	                FROM	merchItemCacheItem
	                GROUP BY itemCacheKey
                ) Q2 ON T1.pk = Q2.itemCacheKey
                WHERE Q2.itemCount > 0 AND
                T1.itemCacheTfKey = @tfKey";

            var sql = new Sql(
                querySql,
                new { @table = table, @start = startDate, @end = endDate, @tfKey = itemCacheTfKey });

            return Database.ExecuteScalar<int>(sql);
        }

        #region Overrides of RepositoryBase<IItemCache>

        /// <summary>
        /// Gets a <see cref="IItemCache"/> by it's key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IItemCache"/>.
        /// </returns>
        protected override IItemCache PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
                .Where(GetBaseWhereClause(), new { Key = key });

            var dto = Database.Fetch<ItemCacheDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new ItemCacheFactory();

            var itemCache = factory.BuildEntity(dto);


            ((ItemCache) itemCache).Items = GetLineItemCollection(itemCache.Key);

            itemCache.ResetDirtyProperties();

            return itemCache;
        }

        /// <summary>
        /// Gets all <see cref="IItemCache"/>.
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IItemCache}"/>.
        /// </returns>
        protected override IEnumerable<IItemCache> PerformGetAll(params Guid[] keys)
        {
            if (keys.Any())
            {
                foreach (var key in keys)
                {
                    yield return Get(key);
                }
            }
            else
            {                
                var dtos = Database.Fetch<ItemCacheDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return Get(dto.Key);
                }
            }
        }

        #endregion
      

        #region Overrides of MerchelloPetaPocoRepositoryBase<IItemCache>

        /// <summary>
        /// Gets base SQL query.
        /// </summary>
        /// <param name="isCount">
        /// The is count.
        /// </param>
        /// <returns>
        /// The <see cref="Sql"/>.
        /// </returns>
        protected override Sql GetBaseQuery(bool isCount)
        {
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
               .From<ItemCacheDto>(SqlSyntax);

            return sql;
        }

        /// <summary>
        /// Gets the base where clause.
        /// </summary>
        /// <returns>
        /// The <see cref="String"/>.
        /// </returns>
        protected override string GetBaseWhereClause()
        {
            return "merchItemCache.pk = @Key";
        }

        /// <summary>
        /// Gets a list of delete clauses.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{String}"/>.
        /// </returns>
        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
                {
                    "DELETE FROM merchItemCacheItem WHERE itemCacheKey = @Key",
                    "DELETE FROM merchItemCache WHERE pk = @Key"
                };

            return list;
        }

        /// <summary>
        /// Adds a new item to the database.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistNewItem(IItemCache entity)
        {
            ((Entity)entity).AddingEntity();

            var factory = new ItemCacheFactory();
            var dto = factory.BuildDto(entity);
            Database.Insert(dto);
            entity.Key = dto.Key;

            _itemCacheLineItemRepository.SaveLineItem(entity.Items, entity.Key);

            entity.ResetDirtyProperties();
        }

        /// <summary>
        /// Updates an existing item in the database.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistUpdatedItem(IItemCache entity)
        {
            ((Entity)entity).UpdatingEntity();

            var factory = new ItemCacheFactory();
            var dto = factory.BuildDto(entity);
            Database.Update(dto);

            _itemCacheLineItemRepository.SaveLineItem(entity.Items, entity.Key);

            entity.ResetDirtyProperties();
        }

        /// <summary>
        /// Deletes an existing item from the database.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistDeletedItem(IItemCache entity)
        {
            var deletes = GetDeleteClauses();
            foreach (var delete in deletes)
            {
                Database.Execute(delete, new { Key = entity.Key });
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="IItemCache"/> by query.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IItemCache}"/>.
        /// </returns>
        protected override IEnumerable<IItemCache> PerformGetByQuery(IQuery<IItemCache> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IItemCache>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<ItemCacheDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key));
        }

        #endregion

        /// <summary>
        /// Gets a page of <see cref="IItemCache"/>
        /// </summary>
        /// <param name="itemCacheTfKey">
        /// The item cache type.
        /// </param>
        /// <param name="startDate">
        /// The start Date.
        /// </param>
        /// <param name="endDate">
        /// The end Date.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="orderExpression">
        /// The sort by field.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{IItemCache}"/>.
        /// </returns>
        private Page<ItemCacheKeyDto> GetPageKeys(
            Guid itemCacheTfKey,
            DateTime startDate,
            DateTime endDate,
            long page,
            long itemsPerPage,
            string orderExpression = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            var sql = new Sql();
            sql.Append("SELECT T1.pk AS itemCacheKey");
            sql.Append("FROM [merchItemCache] T1");
            sql.Append("INNER JOIN [merchCustomer] T2 ON T1.entityKey = T2.pk");
            sql.Append("INNER JOIN (");
            sql.Append("SELECT	itemCacheKey,");
            sql.Append("COUNT(*) AS itemCount");
            sql.Append("FROM [merchItemCacheItem]");
            sql.Append("GROUP BY itemCacheKey");
            sql.Append(") Q1 ON T1.pk = Q1.itemCacheKey");
            sql.Append("WHERE T1.itemCacheTfKey = @tfkey", new { @tfkey = itemCacheTfKey });
            sql.Append("AND	T2.lastActivityDate BETWEEN @start AND @end", new { @start = startDate, @end = endDate });
            sql.Append("AND Q1.itemCount > 0");

            if (!string.IsNullOrEmpty(orderExpression))
            {
                sql.Append(sortDirection == SortDirection.Ascending
                    ? string.Format("ORDER BY {0} ASC", orderExpression)
                    : string.Format("ORDER BY {0} DESC", orderExpression));
            }

            return Database.Page<ItemCacheKeyDto>(page, itemsPerPage, sql);
        }

        /// <summary>
        /// Gets a <see cref="LineItemCollection"/> by an item cache key.
        /// </summary>
        /// <param name="itemCacheKey">
        /// The item cache key.
        /// </param>
        /// <returns>
        /// The <see cref="LineItemCollection"/>.
        /// </returns>
        private LineItemCollection GetLineItemCollection(Guid itemCacheKey)
        {
            var sql = new Sql();
            sql.Select("*")
                .From<ItemCacheItemDto>(SqlSyntax)
                .Where<ItemCacheItemDto>(x => x.ContainerKey == itemCacheKey);

            var dtos = Database.Fetch<ItemCacheItemDto>(sql);

            //var lineItems = _lineItemRepository.GetByContainerId(itemCacheId);

            var factory = new LineItemFactory();
            var collection = new LineItemCollection();
            foreach (var dto in dtos)
            {
                collection.Add(factory.BuildEntity(dto));
            }

            return collection;
        }

        private class ItemCacheKeyDto
        {
            [Column("itemCacheKey")]
            public Guid ItemCacheKey { get; set; }
        }
    }
}
