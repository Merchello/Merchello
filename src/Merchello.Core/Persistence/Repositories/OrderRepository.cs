using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.Rdbms;
using Merchello.Core.Persistence.Querying;
using Merchello.Core.Persistence.Factories;
using Merchello.Core.Persistence.UnitOfWork;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.Querying;

namespace Merchello.Core.Persistence.Repositories
{
    /// <summary>
    /// Represents the OrderRepository
    /// </summary>
    internal class OrderRepository : MerchelloPetaPocoRepositoryBase<IOrder>, IOrderRepository
    {
        private readonly ILineItemRepository _lineItemRepository;

        public OrderRepository(IDatabaseUnitOfWork work, IRuntimeCacheProvider cache, ILineItemRepository lineItemRepository)
            : base(work, cache)
        {
            Mandate.ParameterNotNull(lineItemRepository, "lineItemRepository");

            _lineItemRepository = lineItemRepository;
        }

        protected override IOrder PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
              .Where(GetBaseWhereClause(), new { Key = key });

            var dto = Database.Fetch<OrderDto, OrderIndexDto, OrderStatusDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var lineItems =GetLineItemCollection(key);

            var factory = new OrderFactory(lineItems);
            return factory.BuildEntity(dto);
        }

        protected override IEnumerable<IOrder> PerformGetAll(params Guid[] keys)
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
                ;
                var dtos = Database.Fetch<OrderDto, OrderIndexDto, OrderStatusDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return Get(dto.Key);
                }
            }
        }

        protected override IEnumerable<IOrder> PerformGetByQuery(IQuery<IOrder> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IOrder>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<OrderDto, OrderIndexDto, OrderStatusDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key));
        }

        protected override Sql GetBaseQuery(bool isCount)
        {
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
               .From<OrderDto>()
               .InnerJoin<OrderIndexDto>()
               .On<OrderDto, OrderIndexDto>(left => left.Key, right => right.OrderKey)
               .InnerJoin<OrderStatusDto>()
               .On<OrderDto, OrderStatusDto>(left => left.OrderStatusKey, right => right.Key);

            return sql;
        }

        protected override string GetBaseWhereClause()
        {
            return "merchOrder.pk = @Key";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
            {                
                "DELETE FROM merchOrderItem WHERE orderKey = @Key",
                "DELETE FROM merchOrderIndex WHERE orderKey = @Key",
                "DELETE FROM merchOrder WHERE pk = @Key"
            };

            return list;
        }

        protected override void PersistNewItem(IOrder entity)
        {

            ((Entity)entity).AddingEntity();

            var factory = new OrderFactory(entity.Items);
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);            
            entity.Key = dto.Key;

            Database.Insert(dto.OrderIndexDto);
            ((Order)entity).ExamineId = dto.OrderIndexDto.Id;


            _lineItemRepository.SaveLineItem(entity.Items, entity.Key);

            entity.ResetDirtyProperties();

            RuntimeCache.ClearCacheItem(Cache.CacheKeys.GetEntityCacheKey<IInvoice>(entity.InvoiceKey));
        }

        protected override void PersistUpdatedItem(IOrder entity)
        {
            ((Entity)entity).UpdatingEntity();

            var factory = new OrderFactory(entity.Items);
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            _lineItemRepository.SaveLineItem(entity.Items, entity.Key);

            entity.ResetDirtyProperties();

            RuntimeCache.ClearCacheItem(Cache.CacheKeys.GetEntityCacheKey<IInvoice>(entity.InvoiceKey));
        }

        protected override void PersistDeletedItem(IOrder entity)
        {
            base.PersistDeletedItem(entity);

            RuntimeCache.ClearCacheItem(Cache.CacheKeys.GetEntityCacheKey<IInvoice>(entity.InvoiceKey));
        }

        private LineItemCollection GetLineItemCollection(Guid orderKey)
        {
            var sql = new Sql();
            sql.Select("*")
                .From<OrderItemDto>()
                .Where<OrderItemDto>(x => x.ContainerKey == orderKey);

            var dtos = Database.Fetch<OrderItemDto>(sql);

            var factory = new LineItemFactory();
            var collection = new LineItemCollection();
            foreach (var dto in dtos)
            {
                collection.Add(factory.BuildEntity(dto));
            }

            return collection;
        }
    }
}