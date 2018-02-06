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
    using Merchello.Core.Services;

    using Umbraco.Core;
    using Umbraco.Core.Cache;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Querying;
    using Umbraco.Core.Persistence.SqlSyntax;

    /// <summary>
    /// The ship country repository.
    /// </summary>
    internal class ShipCountryRepository : MerchelloPetaPocoRepositoryBase<IShipCountry>, IShipCountryRepository
    {
        /// <summary>
        /// The store setting service.
        /// </summary>
        private readonly IStoreSettingService _storeSettingService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShipCountryRepository"/> class.
        /// </summary>
        /// <param name="work">
        /// The work.
        /// </param>
        /// <param name="storeSettingService">
        /// The store setting service.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="sqlSyntax">
        /// The SQL syntax.
        /// </param>
        public ShipCountryRepository(IDatabaseUnitOfWork work, IStoreSettingService storeSettingService, ILogger logger, ISqlSyntaxProvider sqlSyntax) 
            : base(work, logger, sqlSyntax)
        {
            Mandate.ParameterNotNull(storeSettingService, "settingsService");

            _storeSettingService = storeSettingService;
        }

        /// <summary>
        /// Determines if a catalog exists for a ship country.
        /// </summary>
        /// <param name="catalogKey">
        /// The catalog key.
        /// </param>
        /// <param name="countryCode">
        /// The country code.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Exists(Guid catalogKey, string countryCode)
        {
            var sql = new Sql();
            sql.Select("*")
                .From<ShipCountryDto>(SqlSyntax)
                .Where<ShipCountryDto>(x => x.CatalogKey == catalogKey && x.CountryCode == countryCode);

            return Database.Fetch<ShipCountryDto>(sql).Any();
        }

        /// <summary>
        /// Gets a <see cref="IShipCountry"/> by it's key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IShipCountry"/>.
        /// </returns>
        protected override IShipCountry PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
                .Where(GetBaseWhereClause(), new { Key = key });

            var dto = Database.Fetch<ShipCountryDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new ShipCountryFactory(_storeSettingService);
            return factory.BuildEntity(dto);
        }

        /// <summary>
        /// Gets all <see cref="IShipCountry"/>.
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IShipCountry}"/>.
        /// </returns>
        protected override IEnumerable<IShipCountry> PerformGetAll(params Guid[] keys)
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
                var factory = new ShipCountryFactory(_storeSettingService);
                var dtos = Database.Fetch<ShipCountryDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return factory.BuildEntity(dto);
                }
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="IShipCountry"/> by query.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IShipCountry}"/>.
        /// </returns>
        protected override IEnumerable<IShipCountry> PerformGetByQuery(IQuery<IShipCountry> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IShipCountry>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<ShipCountryDto>(sql);

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
                .From<ShipCountryDto>(SqlSyntax);

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
            return "merchShipCountry.pk = @Key";
        }

        /// <summary>
        /// Gets the delete clauses.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{String}"/>.
        /// </returns>
        protected override IEnumerable<string> GetDeleteClauses()
        {
            // TODO : RSS - The update in the middle of these delete clauses needs to be refactored - just a quick fix for now
            var list = new List<string>
            {
                "DELETE FROM merchShipRateTier WHERE shipMethodKey IN (SELECT pk FROM merchShipMethod WHERE shipCountryKey = @Key)",                
                "UPDATE merchShipment SET shipMethodKey = NULL WHERE shipMethodKey IN (SELECT pk FROM merchShipMethod WHERE shipCountryKey = @Key)",
                "DELETE FROM merchShipMethod WHERE shipCountryKey = @Key",
                "DELETE FROM merchShipCountry WHERE pk = @Key"
            };

            return list;
        }

        /// <summary>
        /// Saves a new item to the database.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistNewItem(IShipCountry entity)
        {
            // TODO : revisit how this constraint is implemented
            // Assert that a ShipCountry for a given WarehouseCatalog does not already exist with this country code
            if(Exists(entity.CatalogKey, entity.CountryCode)) throw new ConstraintException("A merchShipCountry record already exists with the CatalogKey and CountryCode");

            ((Entity)entity).AddingEntity();

            var factory = new ShipCountryFactory(_storeSettingService);
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
        protected override void PersistUpdatedItem(IShipCountry entity)
        {
            ((Entity)entity).UpdatingEntity();

            var factory = new ShipCountryFactory(_storeSettingService);

            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();
        }       
    }
}