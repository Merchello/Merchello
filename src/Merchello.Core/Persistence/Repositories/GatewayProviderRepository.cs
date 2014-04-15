﻿using System;
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
    internal class GatewayProviderRepository : MerchelloPetaPocoRepositoryBase<IGatewayProvider>, IGatewayProviderRepository
    {

        public GatewayProviderRepository(IDatabaseUnitOfWork work, IRuntimeCacheProvider cache) 
            : base(work, cache)
        {
            
        }

        protected override IGatewayProvider PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
                .Where(GetBaseWhereClause(), new {Key = key});

            var dto = Database.Fetch<GatewayProviderDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new GatewayProviderFactory();
            return factory.BuildEntity(dto);
        }

        protected override IEnumerable<IGatewayProvider> PerformGetAll(params Guid[] keys)
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
                var factory = new GatewayProviderFactory();
                var dtos = Database.Fetch<GatewayProviderDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return factory.BuildEntity(dto);
                }
            }
        }

        protected override IEnumerable<IGatewayProvider> PerformGetByQuery(IQuery<IGatewayProvider> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IGatewayProvider>(sqlClause, query);

            var sql = translator.Translate();

            var dtos = Database.Fetch<GatewayProviderDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key));
        }

        protected override Sql GetBaseQuery(bool isCount)
        {
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
                .From<GatewayProviderDto>();

            return sql;
        }

        protected override string GetBaseWhereClause()
        {
            return "merchGatewayProvider.pk = @Key";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {            
            var list = new List<string>
            {                
                "DELETE FROM merchGatewayProvider WHERE pk = @Key"
            };

            return list;
        }

        protected override void PersistNewItem(IGatewayProvider entity)
        {
            ((Entity)entity).AddingEntity();

            var factory = new GatewayProviderFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);

            entity.Key = dto.Key;

            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(IGatewayProvider entity)
        {
            ((Entity)entity).AddingEntity();

            var factory = new GatewayProviderFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);
            
            entity.ResetDirtyProperties();
        }

        public IEnumerable<IGatewayProvider> GetGatewayProvidersByShipCountryKey(Guid shipCountryKey)
        {
            var sql = new Sql();
            sql.Select("*")
                .From<ShipMethodDto>()
                .InnerJoin<GatewayProviderDto>()
                .On<ShipMethodDto, GatewayProviderDto>(left => left.ProviderKey, right => right.Key)
                .Where<ShipMethodDto>(x => x.ShipCountryKey == shipCountryKey);

            var dtos = Database.Fetch<ShipMethodDto, GatewayProviderDto>(sql);
            var factory = new GatewayProviderFactory();
            return dtos.DistinctBy(x => x.GatewayProviderDto.Key).Select(dto => factory.BuildEntity(dto.GatewayProviderDto));
        }
    }
}