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
    internal class ItemCacheRepository : MerchelloPetaPocoRepositoryBase<IItemCache>, IItemCacheRepository
    {
        private readonly ILineItemRepository _lineItemRepository;


        public ItemCacheRepository(IDatabaseUnitOfWork work, IRuntimeCacheProvider cache, ILineItemRepository lineItemRepository)
            : base(work, cache)
        {
            _lineItemRepository = lineItemRepository;
        }


        #region Overrides of RepositoryBase<IItemCache>


        protected override IItemCache PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
                .Where(GetBaseWhereClause(), new { Key = key });

            var dto = Database.Fetch<ItemCacheDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new ItemCacheFactory();

            var itemCache = factory.BuildEntity(dto);


            ((ItemCache) itemCache).Items = GetLineItemCollection(itemCache.Key);

            itemCache.ResetDirtyProperties();

            return itemCache;
        }

        protected override IEnumerable<IItemCache> PerformGetAll(params Guid[] keys)
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
                var dtos = Database.Fetch<ItemCacheDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return Get(dto.Key);
                }
            }
        }

        #endregion

        private LineItemCollection GetLineItemCollection(Guid itemCacheKey)
        {
            var sql = new Sql();
            sql.Select("*")
                .From<ItemCacheItemDto>()
                .Where<ItemCacheItemDto>(x => x.ContainerKey == itemCacheKey);

            var dtos = Database.Fetch<ItemCacheItemDto>(sql);

            //var lineItems = _lineItemRepository.GetByContainerId(itemCacheId);

            var factory = new LineItemFactory();
            var collection = new LineItemCollection();
            foreach (var dto in dtos)
            {
                collection.Add(factory.BuildEntity(dto));
            }

            return collection;
        }

        #region Overrides of MerchelloPetaPocoRepositoryBase<IItemCache>

        protected override Sql GetBaseQuery(bool isCount)
        {
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
               .From<ItemCacheDto>();

            return sql;
        }

        protected override string GetBaseWhereClause()
        {
            return "merchItemCache.pk = @Key";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
                {
                    "DELETE FROM merchItemCacheItem WHERE itemCacheKey = @Key",
                    "DELETE FROM merchItemCache WHERE pk = @Key"
                };

            return list;
        }

        protected override void PersistNewItem(IItemCache entity)
        {
            ((Entity)entity).AddingEntity();

            var factory = new ItemCacheFactory();
            var dto = factory.BuildDto(entity);
            Database.Insert(dto);
            entity.Key = dto.Key;

            _lineItemRepository.SaveLineItem(entity.Items, entity.Key);

            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(IItemCache entity)
        {
            ((Entity)entity).UpdatingEntity();

            var factory = new ItemCacheFactory();
            var dto = factory.BuildDto(entity);
            Database.Update(dto);

            _lineItemRepository.SaveLineItem(entity.Items, entity.Key);

            entity.ResetDirtyProperties();
        }

        protected override void PersistDeletedItem(IItemCache entity)
        {
            var deletes = GetDeleteClauses();
            foreach (var delete in deletes)
            {
                Database.Execute(delete, new { Key = entity.Key });
            }
        }

        protected override IEnumerable<IItemCache> PerformGetByQuery(IQuery<IItemCache> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IItemCache>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<ItemCacheDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key));
        }        

        #endregion

    }
}
