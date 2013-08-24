using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Persistence.Querying;
using Merchello.Core.Models.Rdbms;
using Merchello.Core.Persistence.Caching;
using Merchello.Core.Persistence.Factories;
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


        protected override Sql GetBaseQuery(bool isCount)
        {
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
               .From("merchAnonymous");

            return sql;
        }

        protected override string GetBaseWhereClause()
        {
            return "merchAnonymous.pk = @Id";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
                {
                    "DELETE FROM merchAnonymous WHERE pk = @Id"
                };

            return list;
        }

        protected override void PersistNewItem(IAnonymousCustomer entity)
        {
            ((AnonymousCustomer)entity).AddingEntity();

            var factory = new AnonymousCustomerFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);

            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(IAnonymousCustomer entity)
        {
            ((AnonymousCustomer)entity).UpdatingEntity();

            var factory = new AnonymousCustomerFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();
        }

        protected override IAnonymousCustomer PerformGet(Guid id)
        {
            var sql = GetBaseQuery(false)
               .Where(GetBaseWhereClause(), new { Id = id });

            var dto = Database.Fetch<AnonymousDto>(sql).FirstOrDefault();

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
                var dtos = Database.Fetch<AnonymousDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return factory.BuildEntity(dto);
                }
            }
        }

        protected override IEnumerable<IAnonymousCustomer> PerformGetByQuery(IQuery<IAnonymousCustomer> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IAnonymousCustomer>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<AnonymousDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key));
        }
    }
}