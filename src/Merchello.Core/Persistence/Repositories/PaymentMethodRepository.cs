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
    /// <summary>
    /// Represents the PaymentMethodRepository
    /// </summary>
    internal class PaymentMethodRepository : MerchelloPetaPocoRepositoryBase<IPaymentMethod>, IPaymentMethodRepository
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentMethodRepository(IDatabaseUnitOfWork work, IRuntimeCacheProvider cache,
            IPaymentRepository paymentRepository)
            : base(work, cache)
        {
            Mandate.ParameterNotNull(paymentRepository, "paymentRepository");
            _paymentRepository = paymentRepository;
        }

        protected override IPaymentMethod PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
              .Where(GetBaseWhereClause(), new { Key = key });

            var dto = Database.Fetch<PaymentMethodDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new PaymentMethodFactory();
            return factory.BuildEntity(dto);
        }

        protected override IEnumerable<IPaymentMethod> PerformGetAll(params Guid[] keys)
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
                var factory = new PaymentMethodFactory();
                var dtos = Database.Fetch<PaymentMethodDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return factory.BuildEntity(dto);
                }
            }
        }

        protected override IEnumerable<IPaymentMethod> PerformGetByQuery(IQuery<IPaymentMethod> query)
        {

            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IPaymentMethod>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<PaymentMethodDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key));
        }

        protected override Sql GetBaseQuery(bool isCount)
        {
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
                .From<PaymentMethodDto>();

            return sql;
        }

        protected override string GetBaseWhereClause()
        {
            return "merchPaymentMethod.pk = @Key";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
            {
                "DELETE FROM merchPaymentMethod WHERE pk = @Key"
            };

            return list;
        }

        protected override void PersistNewItem(IPaymentMethod entity)
        {
            ((Entity)entity).AddingEntity();

            var factory = new PaymentMethodFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);

            entity.Key = dto.Key;

            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(IPaymentMethod entity)
        {
            ((Entity)entity).UpdatingEntity();

            var factory = new PaymentMethodFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();
        }

        protected override void PersistDeletedItem(IPaymentMethod entity)
        {
            Database.Execute("UPDATE merchPayment SET paymentMethodKey = NULL WHERE paymentMethodKey = @Key", new {entity.Key});

            var deletes = GetDeleteClauses();
            foreach (var delete in deletes)
            {
                Database.Execute(delete, new {entity.Key });
            }
        }
    }
}