using System;
using System.Collections.Generic;
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
    internal class GatewayProviderRepository : MerchelloPetaPocoRepositoryBase<IGatewayProviderSettings>, IGatewayProviderRepository
    {

        public GatewayProviderRepository(IDatabaseUnitOfWork work, IRuntimeCacheProvider cache) 
            : base(work, cache)
        {
            
        }

        protected override IGatewayProviderSettings PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
                .Where(GetBaseWhereClause(), new {Key = key});

            var dto = Database.Fetch<GatewayProviderSettingsDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new GatewayProviderSettingsFactory();
            return factory.BuildEntity(dto);
        }

        protected override IEnumerable<IGatewayProviderSettings> PerformGetAll(params Guid[] keys)
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
                var factory = new GatewayProviderSettingsFactory();
                var dtos = Database.Fetch<GatewayProviderSettingsDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return factory.BuildEntity(dto);
                }
            }
        }

        protected override IEnumerable<IGatewayProviderSettings> PerformGetByQuery(IQuery<IGatewayProviderSettings> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IGatewayProviderSettings>(sqlClause, query);

            var sql = translator.Translate();

            var dtos = Database.Fetch<GatewayProviderSettingsDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key));
        }

        protected override Sql GetBaseQuery(bool isCount)
        {
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
                .From<GatewayProviderSettingsDto>();

            return sql;
        }

        protected override string GetBaseWhereClause()
        {
            return "merchGatewayProviderSettings.pk = @Key";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {            
            var list = new List<string>
            {                
                "DELETE FROM merchGatewayProviderSettings WHERE pk = @Key"
            };

            return list;
        }

        protected override void PersistNewItem(IGatewayProviderSettings entity)
        {
            ((Entity)entity).AddingEntity();

            var factory = new GatewayProviderSettingsFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);

            entity.Key = dto.Key;

            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(IGatewayProviderSettings entity)
        {
            ((Entity)entity).AddingEntity();

            var factory = new GatewayProviderSettingsFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);
            
            entity.ResetDirtyProperties();
        }

        public IEnumerable<IGatewayProviderSettings> GetGatewayProvidersByShipCountryKey(Guid shipCountryKey)
        {
            var sql = new Sql();
            sql.Select("*")
                .From<ShipMethodDto>()
                .InnerJoin<GatewayProviderSettingsDto>()
                .On<ShipMethodDto, GatewayProviderSettingsDto>(left => left.ProviderKey, right => right.Key)
                .Where<ShipMethodDto>(x => x.ShipCountryKey == shipCountryKey);

            var dtos = Database.Fetch<ShipMethodDto, GatewayProviderSettingsDto>(sql);
            var factory = new GatewayProviderSettingsFactory();
            return dtos.DistinctBy(x => x.GatewayProviderSettingsDto.Key).Select(dto => factory.BuildEntity(dto.GatewayProviderSettingsDto));
        }
    }
}