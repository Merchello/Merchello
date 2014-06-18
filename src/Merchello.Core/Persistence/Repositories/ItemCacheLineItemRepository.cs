namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Factories;
    using Models;
    using Models.EntityBase;
    using Models.Rdbms;    
    using Querying;    
    using Umbraco.Core;
    using Umbraco.Core.Cache;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Querying;
    using UnitOfWork;

    /// <summary>
    /// Represents a ItemCacheLineItemRepository
    /// </summary>
    internal class ItemCacheLineItemRepository : LineItemRepositoryBase<IItemCacheLineItem>, IItemCacheLineItemRepository
    {
        public ItemCacheLineItemRepository(IDatabaseUnitOfWork work, IRuntimeCacheProvider cache)
            : base(work, cache)
        {            
        }

        public override void Delete(IItemCacheLineItem entity)
        {
            var deletes = GetDeleteClauses();
            foreach (var delete in deletes)
            {
                Database.Execute(delete, new { Key = entity.Key });
            }
        }

        protected override IItemCacheLineItem PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
            .Where(GetBaseWhereClause(), new { Key = key });

            var dto = Database.Fetch<ItemCacheItemDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new ItemCacheLineItemFactory();
            return factory.BuildEntity(dto);
        }

        protected override IEnumerable<IItemCacheLineItem> PerformGetAll(params Guid[] keys)
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
                var factory = new ItemCacheLineItemFactory();
                var dtos = Database.Fetch<ItemCacheItemDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return factory.BuildEntity(dto);
                }
            }
        }

        protected override IEnumerable<IItemCacheLineItem> PerformGetByQuery(IQuery<IItemCacheLineItem> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IItemCacheLineItem>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<ItemCacheItemDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key));
        }

        protected override Sql GetBaseQuery(bool isCount)
        {
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
                .From<ItemCacheItemDto>();

            return sql;
        }

        protected override string GetBaseWhereClause()
        {
            return "merchItemCacheItem.pk = @Key";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
            {
                "DELETE FROM merchItemCacheItem WHERE pk = @Key"
            };

            return list;
        }

        protected override void PersistNewItem(IItemCacheLineItem entity)
        {
            ((Entity)entity).AddingEntity();

            var factory = new ItemCacheLineItemFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);

            entity.Key = dto.Key;

            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(IItemCacheLineItem entity)
        {
            ((Entity)entity).UpdatingEntity();

            var factory = new ItemCacheLineItemFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();
        }


    }
}