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
    internal partial class TransactionRepository : MerchelloPetaPocoRepositoryBase<int, ITransaction>, ITransactionRepository
    {

        public TransactionRepository(IDatabaseUnitOfWork work)
            : base(work)
        {

        }

        public TransactionRepository(IDatabaseUnitOfWork work, IRepositoryCacheProvider cache)
            : base(work, cache)
        {
        }

        #region Overrides of RepositoryBase<ITransaction>


        protected override ITransaction PerformGet(int id)
        {
            var sql = GetBaseQuery(false)
                .Where(GetBaseWhereClause(), new { Id = id });

            var dto = Database.Fetch<TransactionDto, PaymentDto, InvoiceDto, CustomerDto, InvoiceStatusDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new TransactionFactory();

            var transaction = factory.BuildEntity(dto);

            return transaction;
        }

        protected override IEnumerable<ITransaction> PerformGetAll(params int[] ids)
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
                var factory = new TransactionFactory();
                var dtos = Database.Fetch<TransactionDto, PaymentDto, InvoiceDto, CustomerDto, InvoiceStatusDto>(GetBaseQuery(false));
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
                .From<TransactionDto>()
                .InnerJoin<PaymentDto>()
                .On<TransactionDto, PaymentDto>(left => left.PaymentId, right => right.Id)
                .InnerJoin<InvoiceDto>()
                .On<TransactionDto, InvoiceDto>(left => left.InvoiceId, right => right.Id)
                .InnerJoin<CustomerDto>()
                .On<InvoiceDto, CustomerDto>(left => left.CustomerKey, right => right.Key)
                .InnerJoin<InvoiceStatusDto>()
                .On<InvoiceDto, InvoiceStatusDto>(left => left.InvoiceStatusId, right => right.Id);

            return sql;
        }

        protected override string GetBaseWhereClause()
        {
            return "merchTransaction.id = @Id";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
                {
                    "DELETE FROM merchTransaction WHERE TransactionPk = @Id",
                };

            return list;
        }

        protected override void PersistNewItem(ITransaction entity)
        {
            ((IdEntity)entity).AddingEntity();

            var factory = new TransactionFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);
            entity.Id = dto.Id;
            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(ITransaction entity)
        {
            ((IdEntity)entity).UpdatingEntity();

            var factory = new TransactionFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();
        }

        protected override void PersistDeletedItem(ITransaction entity)
        {
            var deletes = GetDeleteClauses();
            foreach (var delete in deletes)
            {
                Database.Execute(delete, new { Id = entity.Id });
            }
        }


        protected override IEnumerable<ITransaction> PerformGetByQuery(IQuery<ITransaction> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<ITransaction>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<TransactionDto, PaymentDto, InvoiceDto, CustomerDto, InvoiceStatusDto>(sql);

            return dtos.DistinctBy(x => x.Id).Select(dto => Get(dto.Id));

        }


        #endregion



    }
}
