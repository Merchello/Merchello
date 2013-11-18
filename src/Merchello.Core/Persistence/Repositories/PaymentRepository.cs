using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.Rdbms;
using Merchello.Core.Persistence.Factories;
using Merchello.Core.Persistence.Querying;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.Querying;
using IDatabaseUnitOfWork = Merchello.Core.Persistence.UnitOfWork.IDatabaseUnitOfWork;

namespace Merchello.Core.Persistence.Repositories
{
    internal class PaymentRepository : MerchelloPetaPocoRepositoryBase<IPayment>, IPaymentRepository
    {

        public PaymentRepository(IDatabaseUnitOfWork work, IRuntimeCacheProvider cache)
            : base(work, cache)
        {
        }

        #region Overrides of RepositoryBase<IPayment>


        protected override IPayment PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
                .Where(GetBaseWhereClause(), new { Key = key });

            var dto = Database.Fetch<PaymentDto, CustomerDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new PaymentFactory();

            var payment = factory.BuildEntity(dto);

            return payment;
        }

        protected override IEnumerable<IPayment> PerformGetAll(params Guid[] keys)
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
                var factory = new PaymentFactory();
                var dtos = Database.Fetch<PaymentDto, CustomerDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return factory.BuildEntity(dto);
                }
            }
        }

        #endregion

        #region Overrides of MerchelloPetaPocoRepositoryBase<IPayment>

        protected override Sql GetBaseQuery(bool isCount)
        {
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
               .From<PaymentDto>()
               .InnerJoin<CustomerDto>()
               .On<PaymentDto, CustomerDto>(left => left.CustomerKey, right => right.Key);              

            return sql;
        }

        protected override string GetBaseWhereClause()
        {
            return "merchPayment.pk = @Key";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
                {
                    "DELETE FROM merchTransaction WHERE paymentKey = @Key",
                    "DELETE FROM merchPayment WHERE pk = @Key"
                };

            return list;
        }

        protected override void PersistNewItem(IPayment entity)
        {
            ((Entity)entity).AddingEntity();

            var factory = new PaymentFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);

            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(IPayment entity)
        {
            ((Entity)entity).UpdatingEntity();

            var factory = new PaymentFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();
        }

        protected override void PersistDeletedItem(IPayment entity)
        {
            var deletes = GetDeleteClauses();
            foreach (var delete in deletes)
            {
                Database.Execute(delete, new { Key = entity.Key });
            }
        }


        protected override IEnumerable<IPayment> PerformGetByQuery(IQuery<IPayment> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IPayment>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<PaymentDto, CustomerDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key));

        }


        #endregion


    }
}
