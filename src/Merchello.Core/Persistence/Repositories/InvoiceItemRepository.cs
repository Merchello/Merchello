using System;
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
    internal partial class InvoiceItemRepository : MerchelloPetaPocoRepositoryBase<int, IInvoiceItem>, IInvoiceItemRepository
    {

        public InvoiceItemRepository(IDatabaseUnitOfWork work)
            : base(work)
        {

        }

        public InvoiceItemRepository(IDatabaseUnitOfWork work, IRepositoryCacheProvider cache)
            : base(work, cache)
        {
        }

        #region Overrides of RepositoryBase<IInvoiceItem>


        protected override IInvoiceItem PerformGet(int id)
        {
            var sql = GetBaseQuery(false)
                .Where(GetBaseWhereClause(), new { Id = id });

            var dto = Database.Fetch<InvoiceItemDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new InvoiceItemFactory();

            var invoiceItem = factory.BuildEntity(dto);

            return invoiceItem;
        }

        protected override IEnumerable<IInvoiceItem> PerformGetAll(params int[] ids)
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
                var factory = new InvoiceItemFactory();
                var dtos = Database.Fetch<InvoiceItemDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return factory.BuildEntity(dto);
                }
            }
        }

        #endregion

        #region Overrides of MerchelloPetaPocoRepositoryBase<IInvoiceItem>

        protected override Sql GetBaseQuery(bool isCount)
        {
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
               .From("merchInvoiceItem");

            return sql;
        }

        protected override string GetBaseWhereClause()
        {
            return "merchInvoiceItem.id = @Id";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
                {
                    "DELETE FROM merchInvoiceItem WHERE id = @Id"
                };

            return list;
        }

        protected override void PersistNewItem(IInvoiceItem entity)
        {
            ((IdEntity)entity).AddingEntity();

            var factory = new InvoiceItemFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);
            entity.Id = dto.Id;
            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(IInvoiceItem entity)
        {
            ((IdEntity)entity).UpdatingEntity();

            var factory = new InvoiceItemFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();
        }

        protected override void PersistDeletedItem(IInvoiceItem entity)
        {
            var deletes = GetDeleteClauses();
            foreach (var delete in deletes)
            {
                Database.Execute(delete, new { Id = entity.Id });
            }
        }


        protected override IEnumerable<IInvoiceItem> PerformGetByQuery(IQuery<IInvoiceItem> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IInvoiceItem>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<InvoiceItemDto>(sql);

            return dtos.DistinctBy(x => x.Id).Select(dto => Get(dto.Id));

        }


        #endregion



    }
}
