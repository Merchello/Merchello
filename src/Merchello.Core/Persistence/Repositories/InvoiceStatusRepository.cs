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
    internal class InvoiceStatusRepository : MerchelloPetaPocoRepositoryBase<IInvoiceStatus>, IInvoiceStatusRepository
    {
        public InvoiceStatusRepository(IDatabaseUnitOfWork work, IRuntimeCacheProvider cache) : base(work, cache)
        { }

        protected override IInvoiceStatus PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
                .Where(GetBaseWhereClause(), new { Key = key });

            var dto = Database.Fetch<InvoiceStatusDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new InvoiceStatusFactory();

            var invoiceStatus = factory.BuildEntity(dto);

            return invoiceStatus;
        }

        protected override IEnumerable<IInvoiceStatus> PerformGetAll(params Guid[] keys)
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
                var factory = new InvoiceStatusFactory();
                var dtos = Database.Fetch<InvoiceStatusDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return factory.BuildEntity(dto);
                }
            }
        }

        protected override IEnumerable<IInvoiceStatus> PerformGetByQuery(IQuery<IInvoiceStatus> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IInvoiceStatus>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<InvoiceStatusDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key));
        }

        protected override Sql GetBaseQuery(bool isCount)
        {
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
               .From<InvoiceStatusDto>();

            return sql;
        }

        protected override string GetBaseWhereClause()
        {
            return "merchInvoiceStatus.pk = @Key";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
                {                    
                    "DELETE FROM merchInvoiceStatus WHERE pk = @Key"
                };

            return list;
        }

        protected override void PersistNewItem(IInvoiceStatus entity)
        {
            ((Entity)entity).AddingEntity();

            var factory = new InvoiceStatusFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);
            entity.Key = dto.Key;
            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(IInvoiceStatus entity)
        {

            ((Entity)entity).UpdatingEntity();

            var factory = new InvoiceStatusFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();
        }
    }
}