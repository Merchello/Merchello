namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Models;
    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Models.Rdbms;
    using Merchello.Core.Persistence.Factories;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Core.Persistence.UnitOfWork;

    using Umbraco.Core;
    using Umbraco.Core.Cache;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Querying;

    /// <summary>
    /// Represents a warehouse catalog repository.
    /// </summary>
    internal class WarehouseCatalogRepository : MerchelloPetaPocoRepositoryBase<IWarehouseCatalog>, IWarehouseCatalogRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WarehouseCatalogRepository"/> class.
        /// </summary>
        /// <param name="work">
        /// The work.
        /// </param>
        /// <param name="cache">
        /// The cache.
        /// </param>
        public WarehouseCatalogRepository(IDatabaseUnitOfWork work, IRuntimeCacheProvider cache)
            : base(work, cache)
        {
        }

        /// <summary>
        /// Gets a collection of <see cref="IWarehouseCatalog"/> by a warehouse key.
        /// </summary>
        /// <param name="warehouseKey">
        /// The warehouse key.
        /// </param>
        /// <returns>
        /// The collection of <see cref="IWarehouseCatalog"/>.
        /// </returns>
        public IEnumerable<IWarehouseCatalog> GetWarehouseCatalogsByWarehouseKey(Guid warehouseKey)
        {
            var query = Querying.Query<IWarehouseCatalog>.Builder.Where(x => x.WarehouseKey == warehouseKey);

            return this.GetByQuery(query);
        }

        /// <summary>
        /// Gets a <see cref="IWarehouseCatalog"/> by it's unique key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IWarehouseCatalog"/>.
        /// </returns>
        protected override IWarehouseCatalog PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
                .Where(GetBaseWhereClause(), new { Key = key });

            var dto = Database.Fetch<WarehouseCatalogDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new WarehouseCatalogFactory();

            var catalog = factory.BuildEntity(dto);

            return catalog;
        }

        /// <summary>
        /// The perform get all.
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// A collection of <see cref="IWarehouseCatalog"/>.
        /// </returns>
        protected override IEnumerable<IWarehouseCatalog> PerformGetAll(params Guid[] keys)
        {
            if (keys.Any())
            {
                foreach (var id in keys)
                {
                    yield return Get(id);
                }
            }
            else
            {
                var factory = new WarehouseCatalogFactory();
                var dtos = Database.Fetch<WarehouseCatalogDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return factory.BuildEntity(dto);
                }
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="IWarehouseCatalog"/> by query.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// A collection of <see cref="IWarehouseCatalog"/>.
        /// </returns>
        protected override IEnumerable<IWarehouseCatalog> PerformGetByQuery(IQuery<IWarehouseCatalog> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IWarehouseCatalog>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<WarehouseCatalogDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key));
        }

        /// <summary>
        /// Gets the base query.
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
               .From<WarehouseCatalogDto>();

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
            return "merchWarehouseCatalog.pk = @Key";
        }

        /// <summary>
        /// Gets a collection delete clauses.
        /// </summary>
        /// <returns>
        /// The collection of delete clauses.
        /// </returns>
        /// <remarks>
        /// This is a complex delete so the ProductVariant (Inventory) of the operation is handled
        /// in the service so that the caching and Lucene indexes do not get messed up.
        /// </remarks>
        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
                {                    
                    "DELETE FROM merchCatalogInventory WHERE catalogKey = @Key",
                    "DELETE FROM merchShipMethod WHERE shipCountryKey IN (SELECT pk FROM merchShipCountry WHERE catalogKey = @Key)",
                    "DELETE FROM merchShipCountry WHERE catalogKey = @Key",
                    "DELETE FROM merchWarehouseCatalog WHERE pk = @Key"
                };

            return list;
        }

        /// <summary>
        /// Persist a new warehouse catalog.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistNewItem(IWarehouseCatalog entity)
        {
            ((Entity)entity).AddingEntity();

            var factory = new WarehouseCatalogFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);
            entity.Key = dto.Key;
            entity.ResetDirtyProperties();

            RuntimeCache.ClearCacheItem(Cache.CacheKeys.GetEntityCacheKey<IWarehouse>(entity.WarehouseKey));
        }

        /// <summary>
        /// Persists an updated warehouse catalog.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistUpdatedItem(IWarehouseCatalog entity)
        {
            ((Entity)entity).AddingEntity();

            var factory = new WarehouseCatalogFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);
            entity.ResetDirtyProperties();

            RuntimeCache.ClearCacheItem(Cache.CacheKeys.GetEntityCacheKey<IWarehouse>(entity.WarehouseKey));
        }

        /// <summary>
        /// Deletes a persisted warehouse catalog
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistDeletedItem(IWarehouseCatalog entity)
        {
            var deletes = GetDeleteClauses();
            foreach (var delete in deletes)
            {
                Database.Execute(delete, new { entity.Key });
            }

            RuntimeCache.ClearCacheItem(Cache.CacheKeys.GetEntityCacheKey<IWarehouse>(entity.WarehouseKey)); 
        }
    }
}