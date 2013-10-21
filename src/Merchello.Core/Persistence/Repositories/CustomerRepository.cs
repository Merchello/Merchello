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
    internal class CustomerRepository : MerchelloPetaPocoRepositoryBase<int, ICustomer>, ICustomerRepository
    {
        
        public CustomerRepository(IDatabaseUnitOfWork work) 
            : base(work)
        {
            
        }

        public CustomerRepository(IDatabaseUnitOfWork work, IRepositoryCacheProvider cache) 
            : base(work, cache)
        {
        }

        #region Overrides of RepositoryBase<ICustomer>
        

        protected override ICustomer PerformGet(int id)
        {
            var sql = GetBaseQuery(false)
                .Where(GetBaseWhereClause(), new {Id = id});


            var dto = Database.Fetch<CustomerDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new CustomerFactory();

            var customer = factory.BuildEntity(dto);

            return customer;
        }

        protected override IEnumerable<ICustomer> PerformGetAll(params int[] ids)
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
            return "merchCustomer.id = @Id";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {
            const string invoiceIdById = "(SELECT id FROM merchInvoice WHERE customerId = @Id)";

            var list = new List<string>
                {
                    // TODO : this needs to be totally refactored
                    "DELETE FROM merchItemCacheItem WHERE ItemCacheId IN (SELECT id FROM merchItemCache WHERE entityKey = (SELECT entityKey FROM merchCustomer WHERE id = @Id))",
                    "DELETE FROM merchItemCache WHERE entityKey = (SELECT entityKey FROM merchCustomer WHERE id = @Id)",
                    "DELETE FROM merchInvoiceItem WHERE invoiceId IN " + invoiceIdById,
                    "DELETE FROM merchShipment WHERE orderId IN (SELECT id FROM merchOrder WHERE customerId = @Id)",
                    "DELETE FROM merchOrderItem WHERE orderId IN (SELECT id FROM merchOrder WHERE customerId = @Id)",
                    "DELETE FROM merchOrder WHERE orderId IN (SELECT id FROM merchOrder WHERE customerId = @Id)",
                    "DELETE FROM merchAppliedPayment WHERE invoiceId IN " + invoiceIdById,
                    "DELETE FROM merchPayment WHERE customerId = @Id",
                    "DELETE FROM merchInvoice WHERE customerId = @Id",
                    "DELETE FROM merchCustomerAddress WHERE customerId = @Id",
                    "DELETE FROM merchCustomer WHERE id = @Id"
                };

            return list;
        }

        protected override void PersistNewItem(ICustomer entity)
        {
            ((IdEntity)entity).AddingEntity();

            var factory = new CustomerFactory();
            var dto = factory.BuildDto(entity);
            
            Database.Insert(dto);
            
            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(ICustomer entity)
        {
            ((IdEntity)entity).UpdatingEntity();

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
                Database.Execute(delete, new { Id = entity.Key });
            }
        }


        protected override IEnumerable<ICustomer> PerformGetByQuery(IQuery<ICustomer> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<ICustomer>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<CustomerDto>(sql);

            return dtos.DistinctBy(x => x.Id).Select(dto => Get(dto.Id));

        }

        #endregion

    }
}
