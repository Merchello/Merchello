using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    internal class ItemCacheFactory : IEntityFactory<IItemCache, ItemCacheDto>
    {

        public IItemCache BuildEntity(ItemCacheDto dto)
        {
            var itemCache = new ItemCache(dto.EntityKey, dto.ItemCacheTfKey)
            {
                Key = dto.Key,
                VersionKey = dto.VersionKey,
                UpdateDate = dto.UpdateDate,
                CreateDate = dto.CreateDate
            };

            itemCache.ResetDirtyProperties();

            return itemCache;
        }

        public ItemCacheDto BuildDto(IItemCache entity)
        {
            var dto = new ItemCacheDto()
            {
                Key = entity.Key,
                EntityKey = entity.EntityKey,
                ItemCacheTfKey = entity.ItemCacheTfKey,
                VersionKey = entity.VersionKey,
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate
            };

            return dto;
        }
    }
}
