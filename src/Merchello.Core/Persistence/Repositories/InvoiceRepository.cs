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
    /// Represents the Invoice Repository
    /// </summary>
    internal class InvoiceRepository : MerchelloPetaPocoRepositoryBase<IInvoice>, IInvoiceRepository
    {
        private readonly ILineItemRepository _lineItemRepository;
        private readonly IOrderRepository _orderRepository;
        
        public InvoiceRepository(IDatabaseUnitOfWork work, IRuntimeCacheProvider cache, ILineItemRepository lineItemRepository, IOrderRepository orderRepository) 
            : base(work, cache)
        {
            Mandate.ParameterNotNull(lineItemRepository, "lineItemRepository");
            Mandate.ParameterNotNull(orderRepository, "orderRepository");

            _lineItemRepository = lineItemRepository;
            _orderRepository = orderRepository;
        }

        protected override IInvoice PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
              .Where(GetBaseWhereClause(), new { Key = key });

            var dto = Database.Fetch<InvoiceDto, InvoiceIndexDto, InvoiceStatusDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            
            var lineItems = GetLineItemCollection(key);
            var orders = GetOrderCollection(key);
            var factory = new InvoiceFactory(lineItems, orders);
            return factory.BuildEntity(dto);
        }

        protected override IEnumerable<IInvoice> PerformGetAll(params Guid[] keys)
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
                var dtos = Database.Fetch<InvoiceDto, InvoiceIndexDto, InvoiceStatusDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return Get(dto.Key);
                }
            }
        }

        protected override IEnumerable<IInvoice> PerformGetByQuery(IQuery<IInvoice> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IInvoice>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<InvoiceDto, InvoiceIndexDto, InvoiceStatusDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key));
        }

        protected override Sql GetBaseQuery(bool isCount)
        {
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
               .From<InvoiceDto>()
               .InnerJoin<InvoiceIndexDto>()
               .On<InvoiceDto, InvoiceIndexDto>(left => left.Key, right => right.InvoiceKey)
               .InnerJoin<InvoiceStatusDto>()
               .On<InvoiceDto, InvoiceStatusDto>(left => left.InvoiceStatusKey, right => right.Key);

            return sql;
        }

        protected override string GetBaseWhereClause()
        {
            return "merchInvoice.pk = @Key";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
            {
                "DELETE FROM merchAppliedPayment WHERE invoiceKey = @Key",
                "DELETE FROM merchInvoiceItem WHERE invoiceKey = @Key",
                "DELETE FROM merchInvoiceIndex WHERE invoiceKey = @Key",
                "DELETE FROM merchInvoice WHERE pk = @Key"
            };

            return list;
        }

        protected override void PersistNewItem(IInvoice entity)
        {
            ((Entity)entity).AddingEntity();

            var factory = new InvoiceFactory(entity.Items, entity.Orders);
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);
            entity.Key = dto.Key;

            Database.Insert(dto.InvoiceIndexDto);
            ((Invoice)entity).ExamineId = dto.InvoiceIndexDto.Id;

            _lineItemRepository.SaveLineItem(entity.Items, entity.Key);

            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(IInvoice entity)
        {
            ((Entity)entity).UpdatingEntity();

            var factory = new InvoiceFactory(entity.Items, entity.Orders);
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            _lineItemRepository.SaveLineItem(entity.Items, entity.Key);

            entity.ResetDirtyProperties();
        }

        private LineItemCollection GetLineItemCollection(Guid invoiceKey)
        {
            var sql = new Sql();
            sql.Select("*")
                .From<InvoiceItemDto>()
                .Where<InvoiceItemDto>(x => x.ContainerKey == invoiceKey);

            var dtos = Database.Fetch<InvoiceItemDto>(sql);

            var factory = new LineItemFactory();
            var collection = new LineItemCollection();
            foreach (var dto in dtos)
            {
                collection.Add(factory.BuildEntity(dto));
            }

            return collection;
        }


        private OrderCollection GetOrderCollection(Guid invoiceKey)
        {
            var query = Querying.Query<IOrder>.Builder.Where(x => x.InvoiceKey == invoiceKey);
            var orders = _orderRepository.GetByQuery(query);
            var collection = new OrderCollection();

            foreach (var order in orders)
            {
                collection.Add(order);
            }

            return collection;            
        }
    }
}