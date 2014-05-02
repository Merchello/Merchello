using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.Rdbms;
using Merchello.Core.Persistence.Factories;
using Merchello.Core.Persistence.UnitOfWork;
using Merchello.Core.Persistence.Querying;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.Querying;

namespace Merchello.Core.Persistence.Repositories
{
    internal class OrderLineItemRepository : LineItemRepositoryBase<IOrderLineItem>, IOrderLineItemRepository
    {
        public OrderLineItemRepository(IDatabaseUnitOfWork work, IRuntimeCacheProvider cache) 
            : base(work, cache)
        { }

        protected override IOrderLineItem PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
            .Where(GetBaseWhereClause(), new { Key = key });

            var dto = Database.Fetch<OrderItemDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new OrderLineItemFactory();
            return factory.BuildEntity(dto);
        }

        protected override IEnumerable<IOrderLineItem> PerformGetAll(params Guid[] keys)
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
                var factory = new OrderLineItemFactory();
                var dtos = Database.Fetch<OrderItemDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return factory.BuildEntity(dto);
                }
            }
        }

        protected override IEnumerable<IOrderLineItem> PerformGetByQuery(IQuery<IOrderLineItem> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IOrderLineItem>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<OrderItemDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key));
        }

        protected override Sql GetBaseQuery(bool isCount)
        {
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
                .From<OrderItemDto>();

            return sql;
        }

        protected override string GetBaseWhereClause()
        {
            return "merchOrderItem.pk = @Key";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
            {
                "DELETE FROM merchOrderItem WHERE pk = @Key"
            };

            return list;
        }

        protected override void PersistNewItem(IOrderLineItem entity)
        {
            ((Entity)entity).AddingEntity();

            var factory = new OrderLineItemFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);

            entity.Key = dto.Key;

            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(IOrderLineItem entity)
        {
            ((Entity)entity).UpdatingEntity();

            var factory = new OrderLineItemFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();
        }
    }
}