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

    using UnitOfWork;

    /// <summary>
    /// Represents a ItemCacheLineItemRepository
    /// </summary>
    internal class ItemCacheLineItemRepository : LineItemRepositoryBase<IItemCacheLineItem>, IItemCacheLineItemRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemCacheLineItemRepository"/> class.
        /// </summary>
        /// <param name="work">
        /// The work.
        /// </param>
        /// <param name="cache">
        /// The cache.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="sqlSyntax">
        /// The SQL syntax.
        /// </param>
        public ItemCacheLineItemRepository(IDatabaseUnitOfWork work, CacheHelper cache, ILogger logger, ISqlSyntaxProvider sqlSyntax)
            : base(work, cache, logger, sqlSyntax)
        {            
        }

        /// <summary>
        /// Overrides the delete.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        public override void Delete(IItemCacheLineItem entity)
        {
            var deletes = GetDeleteClauses();
            foreach (var delete in deletes)
            {
                Database.Execute(delete, new { Key = entity.Key });
            }
        }

        /// <summary>
        /// Gets a <see cref="IItemCacheLineItem"/> by it's keey.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IItemCacheLineItem"/>.
        /// </returns>
        protected override IItemCacheLineItem PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
            .Where(GetBaseWhereClause(), new { Key = key });

            var dto = Database.Fetch<ItemCacheItemDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new ItemCacheLineItemFactory();
            return factory.BuildEntity(dto);
        }

        /// <summary>
        /// Gets a collection of all <see cref="IItemCacheLineItem"/>.
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IItemCacheLineItem}"/>.
        /// </returns>
        protected override IEnumerable<IItemCacheLineItem> PerformGetAll(params Guid[] keys)
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
                var factory = new ItemCacheLineItemFactory();
                var dtos = Database.Fetch<ItemCacheItemDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return factory.BuildEntity(dto);
                }
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="IItemCacheLineItem"/> by query.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IItemCacheLineItem}"/>.
        /// </returns>
        protected override IEnumerable<IItemCacheLineItem> PerformGetByQuery(IQuery<IItemCacheLineItem> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IItemCacheLineItem>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<ItemCacheItemDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key));
        }

        /// <summary>
        /// Gets the base SQL clause.
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
                .From<ItemCacheItemDto>(SqlSyntax);

            return sql;
        }

        /// <summary>
        /// Gets the base where clause.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected override string GetBaseWhereClause()
        {
            return "merchItemCacheItem.pk = @Key";
        }

        /// <summary>
        /// Gets a list of the delete clauses.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{String}"/>.
        /// </returns>
        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
            {
                "DELETE FROM merchItemCacheItem WHERE pk = @Key"
            };

            return list;
        }

        /// <summary>
        /// Adds a new item to the database.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistNewItem(IItemCacheLineItem entity)
        {
            ((Entity)entity).AddingEntity();

            var factory = new ItemCacheLineItemFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);

            entity.Key = dto.Key;

            entity.ResetDirtyProperties();
        }

        /// <summary>
        /// Updates an existing item in the database.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistUpdatedItem(IItemCacheLineItem entity)
        {
            ((Entity)entity).UpdatingEntity();

            var factory = new ItemCacheLineItemFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();
            
            RuntimeCache.ClearCacheItem(Cache.CacheKeys.GetEntityCacheKey<IItemCacheLineItem>(entity.Key));
            RuntimeCache.ClearCacheItem(Cache.CacheKeys.GetEntityCacheKey<ILineItem>(entity.Key));
            RuntimeCache.ClearCacheItem(Cache.CacheKeys.GetEntityCacheKey<IInvoiceLineItem>(entity.Key));
            RuntimeCache.ClearCacheItem(Cache.CacheKeys.GetEntityCacheKey<IOrderLineItem>(entity.Key));
        }
    }
}