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
    internal class OrderStatusRepository: MerchelloPetaPocoRepositoryBase<IOrderStatus>, IOrderStatusRepository
    {
        public OrderStatusRepository(IDatabaseUnitOfWork work, IRuntimeCacheProvider cache) : base(work, cache)
        { }

        protected override IOrderStatus PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
                .Where(GetBaseWhereClause(), new { Key = key });

            var dto = Database.Fetch<OrderStatusDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new OrderStatusFactory();

            var orderStatus = factory.BuildEntity(dto);

            return orderStatus;
        }

        protected override IEnumerable<IOrderStatus> PerformGetAll(params Guid[] keys)
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
                var factory = new OrderStatusFactory();
                var dtos = Database.Fetch<OrderStatusDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return factory.BuildEntity(dto);
                }
            }
        }

        protected override IEnumerable<IOrderStatus> PerformGetByQuery(IQuery<IOrderStatus> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IOrderStatus>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<OrderStatusDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key));
        }

        protected override Sql GetBaseQuery(bool isCount)
        {
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
               .From<OrderStatusDto>();

            return sql;
        }

        protected override string GetBaseWhereClause()
        {
            return "merchOrderStatus.pk = @Key";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
                {                    
                    "DELETE FROM merchOrderStatus WHERE pk = @Key"
                };

            return list;
        }

        protected override void PersistNewItem(IOrderStatus entity)
        {
            ((Entity)entity).AddingEntity();

            var factory = new OrderStatusFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);
            entity.Key = dto.Key;
            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(IOrderStatus entity)
        {

            ((Entity)entity).UpdatingEntity();

            var factory = new OrderStatusFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();
        }
    }
}