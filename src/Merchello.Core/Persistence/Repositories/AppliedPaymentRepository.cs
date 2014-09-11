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
    internal class AppliedPaymentRepository : MerchelloPetaPocoRepositoryBase<IAppliedPayment>, IAppliedPaymentRepository
    {
        public AppliedPaymentRepository(IDatabaseUnitOfWork work, IRuntimeCacheProvider cache) 
            : base(work, cache)
        {
        }

        protected override IAppliedPayment PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
                .Where(GetBaseWhereClause(), new { Key = key });

            var dto = Database.Fetch<AppliedPaymentDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new AppliedPaymentFactory();

            var payment = factory.BuildEntity(dto);

            return payment;
        }

        protected override IEnumerable<IAppliedPayment> PerformGetAll(params Guid[] keys)
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
                var factory = new AppliedPaymentFactory();
                var dtos = Database.Fetch<AppliedPaymentDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return factory.BuildEntity(dto);
                }
            }
        }

        protected override IEnumerable<IAppliedPayment> PerformGetByQuery(IQuery<IAppliedPayment> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IAppliedPayment>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<PaymentDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key));
        }

        protected override Sql GetBaseQuery(bool isCount)
        {
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
               .From<AppliedPaymentDto>();

            return sql;
        }

        protected override string GetBaseWhereClause()
        {
            return "merchAppliedPayment.pk = @Key";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
                {
                    "DELETE FROM merchAppliedPayment WHERE merchAppliedPayment.pk = @Key"
                };

            return list;
        }

        protected override void PersistNewItem(IAppliedPayment entity)
        {
            ((Entity)entity).AddingEntity();

            var factory = new AppliedPaymentFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);
            entity.Key = dto.Key;
            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(IAppliedPayment entity)
        {
            ((Entity)entity).UpdatingEntity();

            var factory = new AppliedPaymentFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();
        }

        protected override void PersistDeletedItem(IAppliedPayment entity)
        {
            var deletes = GetDeleteClauses();
            foreach (var delete in deletes)
            {
                Database.Execute(delete, new { entity.Key });
            }
        }
    }
}