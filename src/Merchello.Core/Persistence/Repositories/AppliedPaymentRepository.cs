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
    internal class AppliedPaymentRepository : MerchelloPetaPocoRepositoryBase<int, IAppliedPayment>, IAppliedPaymentRepository
    {

        public AppliedPaymentRepository(IDatabaseUnitOfWork work)
            : base(work)
        {

        }

        public AppliedPaymentRepository(IDatabaseUnitOfWork work, IRepositoryCacheProvider cache)
            : base(work, cache)
        {
        }

        #region Overrides of RepositoryBase<IAppliedPayment>


        protected override IAppliedPayment PerformGet(int id)
        {
            var sql = GetBaseQuery(false)
                .Where(GetBaseWhereClause(), new { Id = id });

            var dto = Database.Fetch<AppliedPaymentDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new AppliedPaymentFactory();

            var transaction = factory.BuildEntity(dto);

            return transaction;
        }

        protected override IEnumerable<IAppliedPayment> PerformGetAll(params int[] ids)
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
                var factory = new AppliedPaymentFactory();
                var dtos = Database.Fetch<AppliedPaymentDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return factory.BuildEntity(dto);
                }
            }
        }

        #endregion

        #region Overrides of MerchelloPetaPocoRepositoryBase<ITransaction>

        protected override Sql GetBaseQuery(bool isCount)
        {
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
                .From<TransactionDto>();               

            return sql;
        }

        protected override string GetBaseWhereClause()
        {
            return "merchAppliedPayment.id = @Id";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
                {
                    "DELETE FROM merchAppliedPayment WHERE id = @Id",
                };

            return list;
        }

        protected override void PersistNewItem(IAppliedPayment entity)
        {
            ((IdEntity)entity).AddingEntity();

            var factory = new AppliedPaymentFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);
            entity.Id = dto.Id;
            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(IAppliedPayment entity)
        {
            ((IdEntity)entity).UpdatingEntity();

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
                Database.Execute(delete, new { Id = entity.Id });
            }
        }


        protected override IEnumerable<IAppliedPayment> PerformGetByQuery(IQuery<IAppliedPayment> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IAppliedPayment>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<AppliedPaymentDto>(sql);

            return dtos.DistinctBy(x => x.Id).Select(dto => Get(dto.Id));

        }


        #endregion



    }
}
