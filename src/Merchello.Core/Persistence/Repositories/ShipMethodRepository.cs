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

namespace Merchello.Core.Persistence.Repositories
{
    internal class ShipMethodRepository : MerchelloPetaPocoRepositoryBase<IShipMethod>, IShipMethodRepository
    {
        public ShipMethodRepository(IDatabaseUnitOfWork work, IRuntimeCacheProvider cache) 
            : base(work, cache)
        { }

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

        protected override IEnumerable<IShipMethod> PerformGetByQuery(IQuery<IShipMethod> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IShipMethod>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<ShipCountryDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key));
        }

        protected override Sql GetBaseQuery(bool isCount)
        {
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
                .From<ShipMethodDto>();

            return sql;
        }

        protected override string GetBaseWhereClause()
        {
            return "merchShipMethod.pk = @Key";
        }

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

        protected override void PersistNewItem(IShipMethod entity)
        {
            ((Entity)entity).AddingEntity();

            var factory = new ShipMethodFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);

            entity.Key = dto.Key;

            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(IShipMethod entity)
        {
            ((Entity)entity).AddingEntity();

            var factory = new ShipMethodFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();
        }
    }
}