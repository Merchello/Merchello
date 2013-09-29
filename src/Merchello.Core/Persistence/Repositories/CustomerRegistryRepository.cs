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
    internal class CustomerRegistryRepository : MerchelloPetaPocoRepositoryBase<int, ICustomerRegistry>, ICustomerRegistryRepository
    {

        public CustomerRegistryRepository(IDatabaseUnitOfWork work)
            : base(work)
        {

        }

        public CustomerRegistryRepository(IDatabaseUnitOfWork work, IRepositoryCacheProvider cache)
            : base(work, cache)
        {
        }



        #region Overrides of RepositoryBase<ICustomerRegistry>


        protected override ICustomerRegistry PerformGet(int id)
        {
            var sql = GetBaseQuery(false)
                .Where(GetBaseWhereClause(), new { Id = id });

            var dto = Database.Fetch<CustomerRegistryDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new CustomerRegistryFactory();

            var basket = factory.BuildEntity(dto);

            return basket;
        }

        protected override IEnumerable<ICustomerRegistry> PerformGetAll(params int[] ids)
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
                var factory = new CustomerRegistryFactory();
                var dtos = Database.Fetch<CustomerRegistryDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return factory.BuildEntity(dto);
                }
            }
        }

        #endregion

        private ICustomerRegistry CreateCustomerRegistryFromDto(CustomerRegistryDto dto)
        {
            var factory = new CustomerRegistryFactory();
            return null;
        }

        #region Overrides of MerchelloPetaPocoRepositoryBase<ICustomerRegistry>

        protected override Sql GetBaseQuery(bool isCount)
        {
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
               .From("merchCustomerRegistry");

            return sql;
        }

        protected override string GetBaseWhereClause()
        {
            return "merchCustomerRegistry.id = @Id";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
                {
                    "DELETE FROM merchCustomerRegistryItem WHERE basketId = @Id",
                    "DELETE FROM merchCustomerRegistry WHERE id = @Id"
                };

            return list;
        }

        protected override void PersistNewItem(ICustomerRegistry entity)
        {
            ((IdEntity)entity).AddingEntity();

            var factory = new CustomerRegistryFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);
            entity.Id = dto.Id;
            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(ICustomerRegistry entity)
        {
            ((IdEntity)entity).UpdatingEntity();

            var factory = new CustomerRegistryFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();
        }

        protected override void PersistDeletedItem(ICustomerRegistry entity)
        {
            var deletes = GetDeleteClauses();
            foreach (var delete in deletes)
            {
                Database.Execute(delete, new { Id = entity.Id });
            }
        }


        protected override IEnumerable<ICustomerRegistry> PerformGetByQuery(IQuery<ICustomerRegistry> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<ICustomerRegistry>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<CustomerRegistryDto>(sql);

            return dtos.DistinctBy(x => x.Id).Select(dto => Get(dto.Id));

        }

        

        #endregion

    }
}
