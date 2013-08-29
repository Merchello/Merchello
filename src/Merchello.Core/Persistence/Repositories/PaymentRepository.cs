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
    internal class PaymentRepository : MerchelloPetaPocoRepositoryBase<int, IPayment>, IPaymentRepository
    {

        public PaymentRepository(IDatabaseUnitOfWork work)
            : base(work)
        {

        }

        public PaymentRepository(IDatabaseUnitOfWork work, IRepositoryCacheProvider cache)
            : base(work, cache)
        {
        }

        #region Overrides of RepositoryBase<IPayment>


        protected override IPayment PerformGet(int id)
        {
            var sql = GetBaseQuery(false)
                .Where(GetBaseWhereClause(), new { Id = id });

            var dto = Database.Fetch<PaymentDto, CustomerDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new PaymentFactory();

            var payment = factory.BuildEntity(dto);

            return payment;
        }

        protected override IEnumerable<IPayment> PerformGetAll(params int[] ids)
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
               .From("merchPayment")
               .InnerJoin("merchCustomer")
               .On<PaymentDto, CustomerDto>(left => left.CustomerKey, right => right.Key);              

            return sql;
        }

        protected override string GetBaseWhereClause()
        {
            return "merchPayment.id = @Id";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
                {
                    "DELETE FROM merchTransaction WHERE paymentId = @Id",
                    "DELETE FROM merchPayment WHERE id = @Id"
                };

            return list;
        }

        protected override void PersistNewItem(IPayment entity)
        {
            ((IdEntity)entity).AddingEntity();

            var factory = new PaymentFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);
            entity.Id = dto.Id;
            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(IPayment entity)
        {
            ((IdEntity)entity).UpdatingEntity();

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
                Database.Execute(delete, new { Id = entity.Id });
            }
        }


        protected override IEnumerable<IPayment> PerformGetByQuery(IQuery<IPayment> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IPayment>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<PaymentDto, CustomerDto>(sql);

            return dtos.DistinctBy(x => x.Id).Select(dto => Get(dto.Id));

        }


        #endregion



    }
}
