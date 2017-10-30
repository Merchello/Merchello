namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using Merchello.Core.Models;
    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Models.Rdbms;
    using Merchello.Core.Persistence.Factories;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Core.Persistence.UnitOfWork;

    using Umbraco.Core;
    using Umbraco.Core.Cache;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Querying;
    using Umbraco.Core.Persistence.SqlSyntax;

    /// <summary>
    /// The ship method repository.
    /// </summary>
    internal class ShipMethodRepository : MerchelloPetaPocoRepositoryBase<IShipMethod>, IShipMethodRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShipMethodRepository"/> class.
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
        public ShipMethodRepository(
            IDatabaseUnitOfWork work,
            CacheHelper cache,
            ILogger logger,
            ISqlSyntaxProvider sqlSyntax)
            : base(work, cache, logger, sqlSyntax)
        {            
        }

        /// <summary>
        /// Gets a <see cref="IShipMethod"/> by it's key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IShipMethod"/>.
        /// </returns>
        protected override IShipMethod PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
               .Where(GetBaseWhereClause(), new { Key = key });

            var dto = Database.Fetch<ShipMethodDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new ShipMethodFactory();
            return factory.BuildEntity(dto);
        }

        /// <summary>
        /// Gets all <see cref="IShipMethod"/>.
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IShipMethod}"/>.
        /// </returns>
        protected override IEnumerable<IShipMethod> PerformGetAll(params Guid[] keys)
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
                var factory = new ShipMethodFactory();
                var dtos = Database.Fetch<ShipMethodDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return factory.BuildEntity(dto);
                }
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="IShipMethod"/> by query.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IShipMethod}"/>.
        /// </returns>
        protected override IEnumerable<IShipMethod> PerformGetByQuery(IQuery<IShipMethod> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IShipMethod>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<ShipMethodDto>(sql);

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
                .From<ShipMethodDto>(SqlSyntax);

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
            return "merchShipMethod.pk = @Key";
        }

        /// <summary>
        /// Gets the list of delete clauses.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{String}"/>.
        /// </returns>
        protected override IEnumerable<string> GetDeleteClauses()
        {
            // TODO : RSS - The update in the middle of these delete clauses needs to be refactored - just a quick fix for now
            var list = new List<string>
            {
                "DELETE FROM merchShipRateTier WHERE shipMethodKey = @Key",                
                "UPDATE merchShipment SET shipMethodKey = NULL WHERE shipMethodKey = @Key",
                "DELETE FROM merchShipMethod WHERE pk = @Key"
            };

            return list;
        }

        /// <summary>
        /// Saves a new item to the database.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistNewItem(IShipMethod entity)
        {
            // assert a shipmethod does not already exist for this country with this service code
            var query =
                Querying.Query<IShipMethod>.Builder.Where(
                    x => x.ShipCountryKey == entity.ShipCountryKey && x.ServiceCode == entity.ServiceCode);

            if(GetByQuery(query).Any()) throw new ConstraintException("A Shipmethod already exists for this ShipCountry with this ServiceCode");
                
                ((Entity)entity).AddingEntity();

            var factory = new ShipMethodFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);

            entity.Key = dto.Key;

            entity.ResetDirtyProperties();
        }

        /// <summary>
        /// Saves an existing item to the database.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistUpdatedItem(IShipMethod entity)
        {
            ((Entity)entity).AddingEntity();

            var factory = new ShipMethodFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();

            RuntimeCache.ClearCacheItem(Core.Cache.CacheKeys.GetEntityCacheKey<IShipMethod>(entity.Key));
        }
    }
}