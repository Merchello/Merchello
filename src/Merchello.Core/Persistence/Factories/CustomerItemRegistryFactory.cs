using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    internal class CustomerItemRegistryFactory : IEntityFactory<ICustomerItemCache, CustomerItemCacheDto>
    {
        public ICustomerItemCache BuildEntity(CustomerItemCacheDto dto)
        {
            var customerRegistry = new CustomerItemCache(dto.ConsumerKey, dto.ItemCacheTfKey)
            {
                Id = dto.Id,
                UpdateDate = dto.UpdateDate,
                CreateDate = dto.CreateDate
            };

            customerRegistry.ResetDirtyProperties();

            return customerRegistry;
        }

        public CustomerItemCacheDto BuildDto(ICustomerItemCache entity)
        {
            var dto = new CustomerItemCacheDto()
            {
                Id = entity.Id,
                ConsumerKey = entity.ConsumerKey,
                ItemCacheTfKey = entity.ItemCacheTfKey,
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate
            };

            return dto;
        }
    }
}
