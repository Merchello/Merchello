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
    internal class ItemCacheRepository : MerchelloPetaPocoRepositoryBase<int, IItemCache>, IItemCacheRepository
    {
        private readonly ILineItemRepository _lineItemRepository;

        public ItemCacheRepository(IDatabaseUnitOfWork work, ILineItemRepository lineItemRepository)
            : base(work)
        {
            _lineItemRepository = lineItemRepository;
        }

        public ItemCacheRepository(IDatabaseUnitOfWork work, IRepositoryCacheProvider cache, ILineItemRepository lineItemRepository)
            : base(work, cache)
        {
            _lineItemRepository = lineItemRepository;
        }


        #region Overrides of RepositoryBase<ICustomerItemCache>


        protected override IItemCache PerformGet(int id)
        {
            var sql = GetBaseQuery(false)
                .Where(GetBaseWhereClause(), new { Id = id });

            var dto = Database.Fetch<ItemCacheDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new ItemCacheFactory();

            var itemCache = factory.BuildEntity(dto);

            ((ItemCache)itemCache).Items = GetLineItemCollection(itemCache.Id);

            itemCache.ResetDirtyProperties();

            return itemCache;
        }

        protected override IEnumerable<IItemCache> PerformGetAll(params int[] ids)
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
                var dtos = Database.Fetch<ItemCacheDto>(GetBaseQuery(false));
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
                .From<ItemCacheItemDto>()
                .Where<ItemCacheItemDto>(x => x.ContainerId == itemCacheId);

            var dtos = Database.Fetch<ItemCacheItemDto>(sql);

            var factory = new LineItemFactory();
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
               .From<ItemCacheDto>();

            return sql;
        }

        protected override string GetBaseWhereClause()
        {
            return "merchItemCache.id = @Id";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
                {
                    "DELETE FROM merchItemCacheItem WHERE itemCacheId = @Id",
                    "DELETE FROM merchItemCache WHERE id = @Id"
                };

            return list;
        }

        protected override void PersistNewItem(IItemCache entity)
        {
            ((IdEntity)entity).AddingEntity();

            var factory = new ItemCacheFactory();
            var dto = factory.BuildDto(entity);
            Database.Insert(dto);
            entity.Id = dto.Id;

            _lineItemRepository.SaveLineItem(entity.Items);

            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(IItemCache entity)
        {
            ((IdEntity)entity).UpdatingEntity();

            var factory = new ItemCacheFactory();
            var dto = factory.BuildDto(entity);
            Database.Update(dto);

            _lineItemRepository.SaveLineItem(entity.Items);

            entity.ResetDirtyProperties();
        }

        protected override void PersistDeletedItem(IItemCache entity)
        {
            var deletes = GetDeleteClauses();
            foreach (var delete in deletes)
            {
                Database.Execute(delete, new { Id = entity.Id });
            }
        }

        protected override IEnumerable<IItemCache> PerformGetByQuery(IQuery<IItemCache> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IItemCache>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<ItemCacheDto>(sql);

            return dtos.DistinctBy(x => x.Id).Select(dto => Get(dto.Id));
        }        

        #endregion

    }
}
