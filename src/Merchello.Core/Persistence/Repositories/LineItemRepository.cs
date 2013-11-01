using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.Rdbms;
using Merchello.Core.Persistence.Caching;
using Merchello.Core.Persistence.Factories;
using Merchello.Core.Persistence.Querying;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.Querying;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Merchello.Core.Persistence.Repositories
{
    internal class LineItemRepository<TDto> : MerchelloPetaPocoRepositoryBase<int, ILineItem>, ILineItemRepository
        where TDto : ILineItemDto
    {
      
       
        public LineItemRepository(IDatabaseUnitOfWork work) : base(work)
        { }

        public LineItemRepository(IDatabaseUnitOfWork work, IRepositoryCacheProvider cache)
            : base(work, cache)
        { }

        #region Overrides ILineItemRepository
        

        protected override ILineItem PerformGet(int id)
        {
            var sql = GetBaseQuery(false)
               .Where(GetBaseWhereClause(), new { Id = id });

            var dto = (ILineItemDto)Database.Fetch<TDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;


            var lineItem = GetEntity(dto);

            return lineItem;
        }

        protected override IEnumerable<ILineItem> PerformGetAll(params int[] ids)
        {
            if (ids.Any())
            {
                foreach (var id in ids)
                {
                    yield return Get(id);
                }
            }
            else
            {
                var factory = new LineItemFactory();
                var dtos = Database.Fetch<TDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return GetEntity(dto);
                }
            }
        }

   
        protected override IEnumerable<ILineItem> PerformGetByQuery(IQuery<ILineItem> query)
        {
           // convert the IQuery
            var q = query as Querying.Query<ILineItem>;
            if (typeof (TDto) == typeof (InvoiceItemDto))
            {
                var converted = new Querying.Query<IInvoiceLineItem>();
                foreach (var item in q.WhereClauses())
                {
                    converted.WhereClauses().Add(item);
                }
                return PerformGetByQuery(converted);
            }

            if (typeof (TDto) == typeof (OrderItemDto))
            {
                var converted = new Querying.Query<IOrderLineItem>();
                foreach (var item in q.WhereClauses())
                {
                    converted.WhereClauses().Add(item);
                }
                return PerformGetByQuery(converted);
            }

            var final = new Querying.Query<IItemCacheLineItem>();
            foreach (var item in q.WhereClauses())
            {
                final.WhereClauses().Add(item);
            }
            return PerformGetByQuery(final);
        }

        protected IEnumerable<IInvoiceLineItem> PerformGetByQuery(IQuery<IInvoiceLineItem> query)
        {
            var sqlClause = GetBaseQuery(false);

            var translator = new SqlTranslator<IInvoiceLineItem>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<InvoiceItemDto>(sql);

            return dtos.DistinctBy(x => x.Id).Select(dto => (IInvoiceLineItem)Get(dto.Id));
        }

        protected IEnumerable<IOrderLineItem> PerformGetByQuery(IQuery<IOrderLineItem> query)
        {
            var sqlClause = GetBaseQuery(false);

            var translator = new SqlTranslator<IOrderLineItem>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<OrderItemDto>(sql);

            return dtos.DistinctBy(x => x.Id).Select(dto => (IOrderLineItem)Get(dto.Id));
        }

        protected IEnumerable<IItemCacheLineItem> PerformGetByQuery(IQuery<IItemCacheLineItem> query)
        {
            var sqlClause = GetBaseQuery(false);

            var translator = new SqlTranslator<IItemCacheLineItem>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<InvoiceItemDto>(sql);

            return dtos.DistinctBy(x => x.Id).Select(dto => (IItemCacheLineItem)Get(dto.Id));
        }
        
        protected override Sql GetBaseQuery(bool isCount)
        {
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
               .From<TDto>();

            return sql;
        }


        private ILineItem GetEntity(ILineItemDto dto)
        {
            var factory = new LineItemFactory();

            if (typeof(TDto) == typeof(InvoiceItemDto)) return factory.BuildEntity((InvoiceItemDto)dto);
            if (typeof(TDto) == typeof(OrderItemDto)) return factory.BuildEntity((OrderItemDto)dto);
            return factory.BuildEntity((ItemCacheItemDto)dto);
        }

        private ILineItemDto GetDto(ILineItem entity)
        {
            var factory = new LineItemFactory();

            if (typeof(TDto) == typeof(InvoiceItemDto)) return factory.BuildDto((IInvoiceLineItem)entity);
            if (typeof(TDto) == typeof(OrderItemDto)) return factory.BuildDto((IOrderLineItem)entity);
            return factory.BuildDto((IItemCacheLineItem)entity);
        }

        private static string GetMerchTableName()
        {
            return typeof (TDto) == typeof (InvoiceItemDto) ? "merchInvoiceItem"
                : typeof (TDto) == typeof (OrderItemDto) ? "merchOrderItem" 
                : "merchItemCacheItem";
        }

        protected override string GetBaseWhereClause()
        {
            return GetMerchTableName() + ".id = @Id";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {
            return new List<string>()
            {
                "DELETE FROM " + GetMerchTableName() + " WHERE id = @Id"
            };
        }

        protected override void PersistNewItem(ILineItem entity)
        {
            ((IdEntity)entity).AddingEntity();
           
            var dto = GetDto(entity);

            Database.Insert(dto);
            entity.Id = dto.Id;
            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(ILineItem entity)
        {
            ((IdEntity)entity).UpdatingEntity();

            var dto = GetDto(entity);

            Database.Update(dto);
            entity.ResetDirtyProperties();
        }

        #endregion

        public IEnumerable<ILineItem> GetByContainerId(int containerId)
        {
            var query = Querying.Query<ILineItem>.Builder.Where(x => x.ContainerId == containerId);
            return GetByQuery(query);
        }

        public void SaveLineItem(IEnumerable<ILineItem> items)
        {
            var lineItems = items as ILineItem[] ?? items.ToArray();
            if (!lineItems.Any()) return;

            var existing = GetByContainerId(lineItems.First().ContainerId);

            // assert there are no existing items not in the new set of items.  If there are ... delete them
            var toDelete = existing.Where(x => items.Any(item => item.Id != x.Id));
            if (toDelete.Any())
            {
                foreach (var d in toDelete)
                {
                    Delete(d);
                }
            }

            foreach (var item in lineItems)
            {
                SaveLineItem(item);
            }
        }

        public void SaveLineItem(ILineItem item)
        {          
            if (!item.HasIdentity)
            {
                ((IdEntity)item).AddingEntity();
                PersistNewItem(item);
            }
            else
            {
                ((IdEntity)item).UpdatingEntity();
                PersistUpdatedItem(item);
            }            
        }       
    }
}