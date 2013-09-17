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
    internal partial class InvoiceStatusRepository : MerchelloPetaPocoRepositoryBase<int, IInvoiceStatus>, IInvoiceStatusRepository
    {

        public InvoiceStatusRepository(IDatabaseUnitOfWork work)
            : base(work)
        {

        }

        public InvoiceStatusRepository(IDatabaseUnitOfWork work, IRepositoryCacheProvider cache)
            : base(work, cache)
        {
        }

        #region Overrides of RepositoryBase<IInvoiceStatus>


        protected override IInvoiceStatus PerformGet(int id)
        {
            var sql = GetBaseQuery(false)
                .Where(GetBaseWhereClause(), new { Id = id });

            var dto = Database.Fetch<InvoiceStatusDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new InvoiceStatusFactory();

            var invoiceStatus = factory.BuildEntity(dto);

            return invoiceStatus;
        }

        protected override IEnumerable<IInvoiceStatus> PerformGetAll(params int[] ids)
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
                var factory = new InvoiceStatusFactory();
                var dtos = Database.Fetch<InvoiceStatusDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return factory.BuildEntity(dto);
                }
            }
        }

        #endregion

        #region Overrides of MerchelloPetaPocoRepositoryBase<IInvoiceStatus>

        protected override Sql GetBaseQuery(bool isCount)
        {
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
               .From<InvoiceStatusDto>();

            return sql;
        }

        protected override string GetBaseWhereClause()
        {
            return "merchInvoiceStatus.id = @Id";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
                {
                    "DELETE FROM merchInvoiceStatus WHERE Id = @Id"
                };

            return list;
        }

        protected override void PersistNewItem(IInvoiceStatus entity)
        {
            ((IdEntity)entity).AddingEntity();

            var factory = new InvoiceStatusFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);
            entity.Id = dto.Id;
            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(IInvoiceStatus entity)
        {
            ((IdEntity)entity).UpdatingEntity();

            var factory = new InvoiceStatusFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();
        }

        protected override void PersistDeletedItem(IInvoiceStatus entity)
        {
            var deletes = GetDeleteClauses();
            foreach (var delete in deletes)
            {
                Database.Execute(delete, new { Id = entity.Id });
            }
        }


        protected override IEnumerable<IInvoiceStatus> PerformGetByQuery(IQuery<IInvoiceStatus> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IInvoiceStatus>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<InvoiceStatusDto>(sql);

            return dtos.DistinctBy(x => x.Id).Select(dto => Get(dto.Id));

        }


        #endregion



    }
}
