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
                Id = dto.Id,
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
                Id = entity.Id,
                EntityKey = entity.EntityKey,
                ItemCacheTfKey = entity.ItemCacheTfKey,
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate
            };

            return dto;
        }
    }
}
