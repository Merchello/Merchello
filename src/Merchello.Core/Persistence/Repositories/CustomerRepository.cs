﻿using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Models;
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
    internal class CustomerRepository : MerchelloPetaPocoRepositoryBase<Guid, ICustomer>, ICustomerRepository
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
        

        protected override ICustomer PerformGet(Guid id)
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

        protected override IEnumerable<ICustomer> PerformGetAll(params Guid[] ids)
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
        /// Returns 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        internal IPage<ICustomer> GetCustomerByPage(long page, long itemsPerPage)
        {
            var query = new Querying.Query<ICustomer>();
            return PerformGetPageByQuery(page, itemsPerPage, query);
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
            return "merchCustomer.pk = @Id";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {
            const string invoiceIdByKey = "(SELECT id FROM merchInvoice WHERE customerKey = @Id)";

            var list = new List<string>
                {
                    "DELETE FROM merchBasketItem WHERE basketId IN (SELECT id FROM merchBasket WHERE consumerKey = @Id)",
                    "DELETE FROM merchBasket WHERE consumerKey = @Id",
                    "DELETE FROM merchInvoiceItem WHERE invoiceId IN " + invoiceIdByKey,
                    "DELETE FROM merchShipment WHERE invoiceId IN " + invoiceIdByKey,
                    "DELETE FROM merchTransaction WHERE invoiceId IN " + invoiceIdByKey,
                    "DELETE FROM merchPayment WHERE customerKey = @Id",
                    "DELETE FROM merchInvoice WHERE customerKey = @Id",
                    "DELETE FROM merchAddress WHERE customerKey = @Id",
                    "DELETE FROM merchCustomer WHERE pk = @Id"
                };

            return list;
        }

        protected override void PersistNewItem(ICustomer entity)
        {
            ((Customer)entity).AddingEntity();

            var factory = new CustomerFactory();
            var dto = factory.BuildDto(entity);
            
            Database.Insert(dto);
            
            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(ICustomer entity)
        {
            ((Customer)entity).UpdatingEntity();

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

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key));

        }


        protected IPage<ICustomer> PerformGetPageByQuery(long page, long itemsPerPage, IQuery<ICustomer> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<ICustomer>(sqlClause, query);
            var sql = translator.Translate();

            var p = Database.Page<CustomerDto>(page, itemsPerPage, sql);

            return new Models.Page<ICustomer>()
            {
                CurrentPage = p.CurrentPage,
                ItemsPerPage = p.ItemsPerPage,
                TotalItems = p.TotalItems,
                TotalPages = p.TotalPages,
                Items = p.Items.DistinctBy(x => x.Key).Select(dto => Get(dto.Key))
            };
        }

        #endregion




    }
}
