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
    internal class CustomerRepository : MerchelloPetaPocoRepositoryBase<ICustomer>, ICustomerRepository
    {
        

        public CustomerRepository(IDatabaseUnitOfWork work, IRuntimeCacheProvider cache) 
            : base(work, cache)
        {
        }

        #region Overrides of RepositoryBase<ICustomer>
        

        protected override ICustomer PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
                .Where(GetBaseWhereClause(), new {Key = key});


            var dto = Database.Fetch<CustomerDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new CustomerFactory();

            var customer = factory.BuildEntity(dto);

            return customer;
        }

        protected override IEnumerable<ICustomer> PerformGetAll(params Guid[] keys)
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
                var factory = new CustomerFactory();
                var dtos = Database.Fetch<CustomerDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {                    
                    yield return factory.BuildEntity(dto);
                }
            }
        }

        #endregion

        #region Overrides of ICustomerRepository


        //TODO: RSS this needs to be tested
        /// <summary>
        /// Returns a customer based on an Umbraco Member Id
        /// </summary>
        public ICustomer GetByMemberId(int? memberId)
        {
            if (memberId == null) return null;

            var q = new Querying.Query<ICustomer>()
                .Where(c => c.MemberId == memberId);

            return PerformGetByQuery(q).FirstOrDefault();
        }

        /// <summary>
        /// Return a customer based on its entityKey
        /// </summary>
        /// <param name="entityKey"></param>
        /// <returns></returns>
        public ICustomer GetByEntityKey(Guid entityKey)
        {
            Mandate.ParameterCondition(entityKey != Guid.Empty, "entityKey");

            var q = new Querying.Query<ICustomer>()
                .Where(c => c.EntityKey == entityKey);

            return PerformGetByQuery(q).FirstOrDefault();
        }

        #endregion

        #region Overrides of MerchelloPetaPocoRepositoryBase<ICustomer>

        protected override Sql GetBaseQuery(bool isCount)
        {
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
               .From<CustomerDto>();

            return sql;
        }

        protected override string GetBaseWhereClause()
        {
            return "merchCustomer.pk = @Key";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {
            const string invoiceIdByKey = "(SELECT pk FROM merchInvoice WHERE customerKey = @Key)";

            var list = new List<string>
                {
                    // TODO : this needs to be totally refactored
                    "DELETE FROM merchItemCacheItem WHERE ItemCacheKey IN (SELECT pk FROM merchItemCache WHERE entityKey = (SELECT entityKey FROM merchCustomer WHERE pk = @Key))",
                    "DELETE FROM merchItemCache WHERE entityKey = (SELECT entityKey FROM merchCustomer WHERE pk = @Key)",
                    "DELETE FROM merchInvoiceItem WHERE invoiceKey IN " + invoiceIdByKey,
                    "DELETE FROM merchShipment WHERE orderKey IN (SELECT pk FROM merchOrder WHERE customerKey = @Key)",
                    "DELETE FROM merchOrderItem WHERE orderKey IN (SELECT pk FROM merchOrder WHERE customerKey = @Key)",
                    "DELETE FROM merchOrder WHERE orderKey IN (SELECT pk FROM merchOrder WHERE customerKey = @Key)",
                    "DELETE FROM merchAppliedPayment WHERE invoiceKey IN " + invoiceIdByKey,
                    "DELETE FROM merchPayment WHERE customerKey = @Key",
                    "DELETE FROM merchInvoice WHERE customerKey = @Key",
                    "DELETE FROM merchCustomerAddress WHERE customerKey = @Key",
                    "DELETE FROM merchCustomer WHERE pk = @Key"
                };

            return list;
        }

        protected override void PersistNewItem(ICustomer entity)
        {
            ((Entity)entity).AddingEntity();

            var factory = new CustomerFactory();
            var dto = factory.BuildDto(entity);
            
            Database.Insert(dto);
            
            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(ICustomer entity)
        {
            ((Entity)entity).UpdatingEntity();

            var factory = new CustomerFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();
        }

        protected override void PersistDeletedItem(ICustomer entity)
        {
            var deletes = GetDeleteClauses();
            foreach (var delete in deletes)
            {
                Database.Execute(delete, new { Key = entity.Key });
            }
        }


        protected override IEnumerable<ICustomer> PerformGetByQuery(IQuery<ICustomer> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<ICustomer>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<CustomerDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key));

        }

        #endregion

    }
}
