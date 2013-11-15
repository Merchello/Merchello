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
    internal class CustomerAddressRepository : MerchelloPetaPocoRepositoryBase<ICustomerAddress>, ICustomerAddressRepository
    {


        public CustomerAddressRepository(IDatabaseUnitOfWork work, IRuntimeCacheProvider cache)
            : base(work, cache)
        {
        }

        #region Overrides of RepositoryBase<IAddress>


        protected override ICustomerAddress PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
                .Where(GetBaseWhereClause(), new { Key = key });

            var dto = Database.Fetch<CustomerAddressDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new CustomerAddressFactory();

            var address = factory.BuildEntity(dto);

            return address;
        }

        protected override IEnumerable<ICustomerAddress> PerformGetAll(params Guid[] keys)
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
                var factory = new CustomerAddressFactory();
                var dtos = Database.Fetch<CustomerAddressDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return factory.BuildEntity(dto);
                }
            }
        }

        #endregion

        #region Overrides of MerchelloPetaPocoRepositoryBase<IAddress>

        protected override Sql GetBaseQuery(bool isCount)
        {
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
               .From("merchAddress");

            return sql;
        }

        protected override string GetBaseWhereClause()
        {
            return "merchAddress.pk = @Key";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
                {
                    "DELETE FROM merchAddress WHERE pk = @Key",
                };

            return list;
        }

        protected override void PersistNewItem(ICustomerAddress entity)
        {
            ((Entity)entity).AddingEntity();

            var factory = new CustomerAddressFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);
            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(ICustomerAddress entity)
        {
            ((Entity)entity).UpdatingEntity();

            var factory = new CustomerAddressFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();
        }

        protected override void PersistDeletedItem(ICustomerAddress entity)
        {
            var deletes = GetDeleteClauses();
            foreach (var delete in deletes)
            {
                Database.Execute(delete, new { Key = entity.Key });
            }
        }


        protected override IEnumerable<ICustomerAddress> PerformGetByQuery(IQuery<ICustomerAddress> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<ICustomerAddress>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<CustomerAddressDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key));

        }


        #endregion



    }
}
