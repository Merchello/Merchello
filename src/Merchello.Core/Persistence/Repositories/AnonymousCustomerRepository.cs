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
    internal class AnonymousCustomerRepository : MerchelloPetaPocoRepositoryBase<Guid, IAnonymousCustomer>, IAnonymousCustomerRepository
    {

        public AnonymousCustomerRepository(IDatabaseUnitOfWork work)
            : base(work)
        {

        }

        public AnonymousCustomerRepository(IDatabaseUnitOfWork work, IRepositoryCacheProvider cache)
            : base(work, cache)
        {
        }

        #region Overrides of RepositoryBase<IAnonymous>


        protected override IAnonymousCustomer PerformGet(Guid id)
        {
            var sql = GetBaseQuery(false)
                .Where(GetBaseWhereClause(), new { Id = id });

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
            return "merchAnonymousCustomer.pk = @Id";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
                {
                    "DELETE FROM merchItemCacheItem WHERE itemCacheId IN (SELECT id FROM merchItemCache WHERE entityKey = @Id)",
                    "DELETE FROM merchItemCache WHERE entityKey = @Id",
                    "DELETE FROM merchAnonymousCustomer WHERE pk = @Id",
                };

            return list;
        }

        protected override void PersistNewItem(IAnonymousCustomer entity)
        {
            ((KeyEntity)entity).AddingEntity();

            var factory = new AnonymousCustomerFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);
            entity.Key = dto.Key;
            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(IAnonymousCustomer entity)
        {
            ((KeyEntity)entity).UpdatingEntity();

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
                Database.Execute(delete, new { Id = entity.Key });
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
