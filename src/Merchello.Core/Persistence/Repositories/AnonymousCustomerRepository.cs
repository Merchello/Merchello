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
using Merchello.Core.Persistence.UnitOfWork;

namespace Merchello.Core.Persistence.Repositories
{
    internal class AnonymousCustomerRepository : MerchelloPetaPocoRepositoryBase<IAnonymousCustomer>, IAnonymousCustomerRepository
    {


        public AnonymousCustomerRepository(IDatabaseUnitOfWork work, IRuntimeCacheProvider cache)
            : base(work, cache)
        {
        }

        #region Overrides of RepositoryBase<IAnonymous>


        protected override IAnonymousCustomer PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
                .Where(GetBaseWhereClause(), new { Key = key });

            var dto = Database.Fetch<AnonymousCustomerDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new AnonymousCustomerFactory();

            var anonymous = factory.BuildEntity(dto);

            return anonymous;
        }

        protected override IEnumerable<IAnonymousCustomer> PerformGetAll(params Guid[] ids)
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
                var factory = new AnonymousCustomerFactory();
                var dtos = Database.Fetch<AnonymousCustomerDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return factory.BuildEntity(dto);
                }
            }
        }

        #endregion

        #region Overrides of MerchelloPetaPocoRepositoryBase<IAnonymous>

        protected override Sql GetBaseQuery(bool isCount)
        {
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
               .From<AnonymousCustomerDto>();

            return sql;
        }

        protected override string GetBaseWhereClause()
        {
            return "merchAnonymousCustomer.pk = @Key";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
                {
                    "DELETE FROM merchItemCacheItem WHERE itemCacheKey IN (SELECT pk FROM merchItemCache WHERE entityKey = @Key)",
                    "DELETE FROM merchItemCache WHERE entityKey = @Key",
                    "DELETE FROM merchAnonymousCustomer WHERE pk = @Key",
                };

            return list;
        }

        protected override void PersistNewItem(IAnonymousCustomer entity)
        {
            ((Entity)entity).AddingEntity();

            var factory = new AnonymousCustomerFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);
            entity.Key = dto.Key;
            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(IAnonymousCustomer entity)
        {
            ((Entity)entity).UpdatingEntity();

            var factory = new AnonymousCustomerFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();
        }

        protected override void PersistDeletedItem(IAnonymousCustomer entity)
        {
            var deletes = GetDeleteClauses();
            foreach (var delete in deletes)
            {
                Database.Execute(delete, new { Key = entity.Key });
            }
        }


        protected override IEnumerable<IAnonymousCustomer> PerformGetByQuery(IQuery<IAnonymousCustomer> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IAnonymousCustomer>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<AnonymousCustomerDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key));

        }


        #endregion



    }
}
