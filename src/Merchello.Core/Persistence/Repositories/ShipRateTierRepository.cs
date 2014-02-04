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
    /// <summary>
    /// Represents the ShipRateTierRepository
    /// </summary>
    internal class ShipRateTierRepository : MerchelloPetaPocoRepositoryBase<IShipRateTier>, IShipRateTierRepository
    {
        public ShipRateTierRepository(IDatabaseUnitOfWork work, IRuntimeCacheProvider cache) 
            : base(work, cache)
        { }

        protected override IShipRateTier PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
              .Where(GetBaseWhereClause(), new { Key = key });

            var dto = Database.Fetch<ShipRateTierDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new ShipRateTierFactory();
            return factory.BuildEntity(dto);
        }

        protected override IEnumerable<IShipRateTier> PerformGetAll(params Guid[] keys)
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
                var factory = new ShipRateTierFactory();
                var dtos = Database.Fetch<ShipRateTierDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return factory.BuildEntity(dto);
                }
            }
        }

        protected override IEnumerable<IShipRateTier> PerformGetByQuery(IQuery<IShipRateTier> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IShipRateTier>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<ShipRateTierDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key));
        }

        protected override Sql GetBaseQuery(bool isCount)
        {
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
                .From<ShipRateTierDto>();

            return sql;
        }

        protected override string GetBaseWhereClause()
        {
            return "merchShipRateTier.pk = @Key";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
            {
                "DELETE FROM merchShipRateTier WHERE pk = @Key"
            };

            return list;
        }

        protected override void PersistNewItem(IShipRateTier entity)
        {
            ((Entity)entity).AddingEntity();

            var factory = new ShipRateTierFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);

            entity.Key = dto.Key;

            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(IShipRateTier entity)
        {
            ((Entity)entity).AddingEntity();

            var factory = new ShipRateTierFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();
        }
    }
}