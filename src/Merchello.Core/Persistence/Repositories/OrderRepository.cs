namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Factories;
    using Models;
    using Models.EntityBase;
    using Models.Rdbms;    
    using Querying;    
    using Umbraco.Core;
    using Umbraco.Core.Cache;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Querying;
    using UnitOfWork;

    /// <summary>
    /// Represents the OrderRepository
    /// </summary>
    internal class OrderRepository : PagedRepositoryBase<IOrder, OrderDto>, IOrderRepository
    {
        private readonly ILineItemRepositoryBase<IOrderLineItem> _orderLineItemRepository;

        public OrderRepository(IDatabaseUnitOfWork work, IRuntimeCacheProvider cache, ILineItemRepositoryBase<IOrderLineItem> orderLineItemRepository)
            : base(work, cache)
        {
            Mandate.ParameterNotNull(orderLineItemRepository, "lineItemRepository");

            _orderLineItemRepository = orderLineItemRepository;
        }

        public override Page<Guid> SearchKeys(
            string searchTerm,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            throw new NotImplementedException();
        }

        protected override IOrder PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
              .Where(GetBaseWhereClause(), new { Key = key });

            var dto = Database.Fetch<OrderDto, OrderIndexDto, OrderStatusDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var lineItems = GetLineItemCollection(key);

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


            _orderLineItemRepository.SaveLineItem(entity.Items, entity.Key);

            entity.ResetDirtyProperties();

            RuntimeCache.ClearCacheItem(Cache.CacheKeys.GetEntityCacheKey<IInvoice>(entity.InvoiceKey));
        }

        protected override void PersistUpdatedItem(IOrder entity)
        {
            ((Entity)entity).UpdatingEntity();

            var factory = new OrderFactory(entity.Items);
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            _orderLineItemRepository.SaveLineItem(entity.Items, entity.Key);

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

        /// <summary>
        /// The get max document number.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetMaxDocumentNumber()
        {

            var value = Database.ExecuteScalar<object>("SELECT TOP 1 orderNumber FROM merchOrder ORDER BY orderNumber DESC");
            return value == null ? 0 : int.Parse(value.ToString());
        }
    }
}