using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;
using Merchello.Core.Persistence.Caching;
using Merchello.Core.Persistence.Factories;
using Umbraco.Core.Models.EntityBase;
using Umbraco.Core.Persistence;

using Umbraco.Core.Persistence.UnitOfWork;

namespace Merchello.Core.Persistence.Respositories
{
    internal class CustomerRepository : SqlPetaPocoRepositoryBase<Guid, ICustomer>, ICustomerRepository
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
                var dtos = Database.Fetch<CustomerDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return Get(dto.Pk);
                }
            }
        }

        #endregion

        #region Overrides of PetaPocoRepositoryBase<ICustomer>

        protected override Sql GetBaseQuery(bool isCount)
        {
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
               .From("merchCustomer");

            return sql;
        }

        protected override string GetBaseWhereClause()
        {
            return "merchCustomer.pk = @Id";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {
            const string invoiceIdByPk = "(SELECT id FROM merchInvoice WHERE customerPk = @Id)";

            var list = new List<string>
                {
                    "DELETE FROM merchInvoiceItem WHERE invoiceId = " + invoiceIdByPk,
                    "DELETE FROM merchShipment WHERE invoiceId = " + invoiceIdByPk,
                    "DELETE FROM merchPayment WHERE invoiceId = " + invoiceIdByPk,
                    "DELETE FROM merchInvoice WHERE customerPk = @Id",
                    "DELETE FROM merchAddress WHERE customerPk = @Id",
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

        #endregion


    }
}
