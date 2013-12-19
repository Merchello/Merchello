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
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.Querying;

namespace Merchello.Core.Persistence.Repositories
{
    internal class ShipCountryRepository : MerchelloPetaPocoRepositoryBase<IShipCountry>, IShipCountryRepository
    {
        private readonly ISettingsService _settingsService;

        public ShipCountryRepository(IDatabaseUnitOfWork work, IRuntimeCacheProvider cache, ISettingsService settingsService) 
            : base(work, cache)
        {
            Mandate.ParameterNotNull(settingsService, "settingsService");

            _settingsService = settingsService;
        }     

        protected override IShipCountry PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
                .Where(GetBaseWhereClause(), new { Key = key });

            var dto = Database.Fetch<ShipCountryDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new ShipCountryFactory(_settingsService);
            return factory.BuildEntity(dto);
        }

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
                var factory = new ShipCountryFactory(_settingsService);
                var dtos = Database.Fetch<ShipCountryDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return factory.BuildEntity(dto);
                }
            }
        }

        
        protected override IEnumerable<IShipCountry> PerformGetByQuery(IQuery<IShipCountry> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IShipCountry>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<ShipCountryDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key));

        }

        protected override Sql GetBaseQuery(bool isCount)
        {
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
                .From<ShipCountryDto>();

            return sql;
        }

        protected override string GetBaseWhereClause()
        {
            return "merchShipCountry.pk = @Key";
        }


        protected override IEnumerable<string> GetDeleteClauses()
        {
            // TODO : RSS - The update in the middle of these delete clauses needs to be refactored - just a quick fix for now
            var list = new List<string>
            {
                "DELETE FROM merchShipRateTier WHERE shipMethodKey IN (SELECT pk FROM merchShipMethod WHERE providerKey IN (SELECT pk FROM merchGatewayProvider WHERE pk IN (SELECT gatewayProviderKey FROM merchGatewayProvider2ShipCountry WHERE shipCountryKey = @Key)))",                
                "UPDATE merchShipment SET shipMethodKey = NULL WHERE shipMethodKey IN (SELECT pk FROM merchShipMethod WHERE providerKey IN (SELECT pk FROM merchGatewayProvider WHERE pk IN (SELECT gatewayProviderKey FROM merchGatewayProvider2ShipCountry WHERE shipCountryKey = @Key)))",
                "DELETE FROM merchShipMethod WHERE providerKey IN (SELECT pk FROM merchGatewayProvider WHERE pk IN (SELECT gatewayProviderKey FROM merchGatewayProvider2ShipCountry WHERE shipCountryKey = @Key))",
                "DELETE FROM merchGatewayProvider2ShipCountry WHERE shipCountryKey = @Key",
                "DELETE FROM merchShipCountry WHERE pk = @Key"
            };

            return list;
        }

        protected override void PersistNewItem(IShipCountry entity)
        {
            // TODO : revisit how this constraint is implemented
            // Assert that a ShipCountry for a given WarehouseCatalog does not already exist with this country code
            if(Exists(entity.CatalogKey, entity.CountryCode)) throw new ConstraintException("A merchShipCountry record already exists with the CatalogKey and CountryCode");

            ((Entity)entity).AddingEntity();

            var factory = new ShipCountryFactory(_settingsService);
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);

            entity.Key = dto.Key;

            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(IShipCountry entity)
        {
            ((Entity)entity).UpdatingEntity();

            var factory = new ShipCountryFactory(_settingsService);

            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();
        }

        public bool Exists(Guid catalogKey, string countryCode)
        {
            var sql = new Sql();
            sql.Select("*")
                .From<ShipCountryDto>()
                .Where<ShipCountryDto>(x => x.CatalogKey == catalogKey && x.CountryCode == countryCode);

            return Database.Fetch<ShipCountryDto>(sql).Any();
        }
    }
}