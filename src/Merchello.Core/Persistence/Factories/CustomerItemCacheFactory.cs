using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    internal class CustomerItemCacheFactory : IEntityFactory<ICustomerItemCache, CustomerItemCacheDto>
    {

        public ICustomerItemCache BuildEntity(CustomerItemCacheDto dto)
        {
            var itemCache = new CustomerItemCache(dto.CustomerKey, dto.ItemCacheTfKey)
            {
                Id = dto.Id,
                UpdateDate = dto.UpdateDate,
                CreateDate = dto.CreateDate
            };

            itemCache.ResetDirtyProperties();

            return itemCache;
        }

        public CustomerItemCacheDto BuildDto(ICustomerItemCache entity)
        {
            var dto = new CustomerItemCacheDto()
            {
                Id = entity.Id,
                CustomerKey = entity.CustomerKey,
                ItemCacheTfKey = entity.ItemCacheTfKey,
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate
            };

            return dto;
        }
    }
}
