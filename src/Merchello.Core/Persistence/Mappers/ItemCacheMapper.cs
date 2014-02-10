using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Mappers
{
    /// <summary>
    /// Represents a <see cref="ItemCache"/> to DTO mapper used to translate the properties of the public api 
    /// implementation to that of the database's DTO as sql: [tableName].[columnName].
    /// </summary>
    internal sealed class ItemCacheMapper : MerchelloBaseMapper
    {
        public ItemCacheMapper()
        {
            BuildMap();
        }

        internal override void BuildMap()
        {
            if (!PropertyInfoCache.IsEmpty) return;

            CacheMap<ItemCache, ItemCacheDto>(src => src.Key, dto => dto.Key);
            CacheMap<ItemCache, ItemCacheDto>(src => src.ItemCacheTfKey, dto => dto.ItemCacheTfKey);
            CacheMap<ItemCache, ItemCacheDto>(src => src.EntityKey, dto => dto.EntityKey);
            CacheMap<ItemCache, ItemCacheDto>(src => src.VersionKey, dto => dto.VersionKey);
            CacheMap<ItemCache, ItemCacheDto>(src => src.CreateDate, dto => dto.CreateDate);
            CacheMap<ItemCache, ItemCacheDto>(src => src.UpdateDate, dto => dto.UpdateDate);
        }
    }
}
