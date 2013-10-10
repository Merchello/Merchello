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
    internal class AddressRepository : MerchelloPetaPocoRepositoryBase<int, IAddress>, IAddressRepository
    {

        public AddressRepository(IDatabaseUnitOfWork work)
            : base(work)
        {

        }

        public AddressRepository(IDatabaseUnitOfWork work, IRepositoryCacheProvider cache)
            : base(work, cache)
        {
        }

        #region Overrides of RepositoryBase<IAddress>


        protected override IAddress PerformGet(int id)
        {
            var sql = GetBaseQuery(false)
                .Where(GetBaseWhereClause(), new { Id = id });

            var dto = Database.Fetch<CustomerAddressDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new AddressFactory();

            var address = factory.BuildEntity(dto);

            return address;
        }

        protected override IEnumerable<IAddress> PerformGetAll(params int[] ids)
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
                var factory = new AddressFactory();
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
            return "merchAddress.id = @Id";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
                {
                    "DELETE FROM merchAddress WHERE Id = @Id",
                };

            return list;
        }

        protected override void PersistNewItem(IAddress entity)
        {
            ((IdEntity)entity).AddingEntity();

            var factory = new AddressFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);
            entity.Id = dto.Id;
            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(IAddress entity)
        {
            ((IdEntity)entity).UpdatingEntity();

            var factory = new AddressFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();
        }

        protected override void PersistDeletedItem(IAddress entity)
        {
            var deletes = GetDeleteClauses();
            foreach (var delete in deletes)
            {
                Database.Execute(delete, new { Id = entity.Id });
            }
        }


        protected override IEnumerable<IAddress> PerformGetByQuery(IQuery<IAddress> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IAddress>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<CustomerAddressDto>(sql);

            return dtos.DistinctBy(x => x.Id).Select(dto => Get(dto.Id));

        }


        #endregion



    }
}
