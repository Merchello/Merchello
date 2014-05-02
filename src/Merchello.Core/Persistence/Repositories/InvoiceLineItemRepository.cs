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
    internal class InvoiceLineItemRepository : LineItemRepositoryBase<IInvoiceLineItem>, IInvoiceLineItemRepository
    {
        public InvoiceLineItemRepository(IDatabaseUnitOfWork work, IRuntimeCacheProvider cache) 
            : base(work, cache)
        { }

        protected override IInvoiceLineItem PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
             .Where(GetBaseWhereClause(), new { Key = key });

            var dto = Database.Fetch<InvoiceItemDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new InvoiceLineItemFactory();
            return factory.BuildEntity(dto);
        }

        protected override IEnumerable<IInvoiceLineItem> PerformGetAll(params Guid[] keys)
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
                var factory = new InvoiceLineItemFactory();
                var dtos = Database.Fetch<InvoiceItemDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return factory.BuildEntity(dto);
                }
            }
        }

        protected override IEnumerable<IInvoiceLineItem> PerformGetByQuery(IQuery<IInvoiceLineItem> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IInvoiceLineItem>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<InvoiceItemDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key));
        }

        protected override Sql GetBaseQuery(bool isCount)
        {
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
                .From<InvoiceItemDto>();

            return sql;
        }

        protected override string GetBaseWhereClause()
        {
            return "merchInvoiceItem.pk = @Key";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
            {
                "DELETE FROM merchInvoiceItem WHERE pk = @Key"
            };

            return list;
        }

        protected override void PersistNewItem(IInvoiceLineItem entity)
        {
            ((Entity)entity).AddingEntity();

            var factory = new InvoiceLineItemFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);

            entity.Key = dto.Key;

            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(IInvoiceLineItem entity)
        {
            ((Entity)entity).UpdatingEntity();

            var factory = new InvoiceLineItemFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();
        }
    }
}