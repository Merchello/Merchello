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
    internal class CustomerItemCacheRepository : MerchelloPetaPocoRepositoryBase<int, ICustomerItemCache>, ICustomerItemCacheRepository
    {

        public CustomerItemCacheRepository(IDatabaseUnitOfWork work)
            : base(work)
        {

        }

        public CustomerItemCacheRepository(IDatabaseUnitOfWork work, IRepositoryCacheProvider cache)
            : base(work, cache)
        {
        }


        #region Overrides of RepositoryBase<ICustomerRegistry>


        protected override ICustomerItemCache PerformGet(int id)
        {
            var sql = GetBaseQuery(false)
                .Where(GetBaseWhereClause(), new { Id = id });

            var dto = Database.Fetch<CustomerItemCacheDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new CustomerItemCacheFactory();

            var itemCache = factory.BuildEntity(dto);

            ((CustomerItemCache)itemCache).Items = GetLineItemCollection(itemCache.Id);

            itemCache.ResetDirtyProperties();

            return itemCache;
        }

        protected override IEnumerable<ICustomerItemCache> PerformGetAll(params int[] ids)
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
                var dtos = Database.Fetch<CustomerItemCacheDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return Get(dto.Id);
                }
            }
        }

        #endregion

        private LineItemCollection GetLineItemCollection(int itemCacheId)
        {
            var sql = new Sql();
            sql.Select("*")
                .From<CustomerItemCacheItemDto>()
                .Where<CustomerItemCacheItemDto>(x => x.ItemCacheId == itemCacheId);

            var dtos = Database.Fetch<CustomerItemCacheItemDto>(sql);

            var factory = new CustomerItemCacheLineItemFactory();
            var collection = new LineItemCollection();
            foreach (var dto in dtos)
            {
                collection.Add(factory.BuildEntity(dto));
            }

            return collection;
        }

        #region Overrides of MerchelloPetaPocoRepositoryBase<ICustomerRegistry>

        protected override Sql GetBaseQuery(bool isCount)
        {
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
               .From("merchCustomerItemCache");

            return sql;
        }

        protected override string GetBaseWhereClause()
        {
            return "merchCustomerItemCache.id = @Id";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
                {
                    "DELETE FROM merchCustomerItemCacheItem WHERE itemCacheId = @Id",
                    "DELETE FROM merchCustomerItemCache WHERE id = @Id"
                };

            return list;
        }

        protected override void PersistNewItem(ICustomerItemCache entity)
        {
            ((IdEntity)entity).AddingEntity();

            var factory = new CustomerItemCacheFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);
            entity.Id = dto.Id;
            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(ICustomerItemCache entity)
        {
            ((IdEntity)entity).UpdatingEntity();

            var factory = new CustomerItemCacheFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();
        }

        protected override void PersistDeletedItem(ICustomerItemCache entity)
        {
            var deletes = GetDeleteClauses();
            foreach (var delete in deletes)
            {
                Database.Execute(delete, new { Id = entity.Id });
            }
        }

        protected override IEnumerable<ICustomerItemCache> PerformGetByQuery(IQuery<ICustomerItemCache> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<ICustomerItemCache>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<CustomerItemCacheDto>(sql);

            return dtos.DistinctBy(x => x.Id).Select(dto => Get(dto.Id));
        }        

        #endregion

    }
}
