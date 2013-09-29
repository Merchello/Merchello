using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    internal class CustomerRegistryFactory : IEntityFactory<ICustomerRegistry, CustomerRegistryDto>
    {
        public ICustomerRegistry BuildEntity(CustomerRegistryDto dto)
        {
            var customerRegistry = new CustomerRegistry(dto.ConsumerKey, dto.RegistryTfKey)
            {
                Id = dto.Id,
                UpdateDate = dto.UpdateDate,
                CreateDate = dto.CreateDate
            };

            customerRegistry.ResetDirtyProperties();

            return customerRegistry;
        }

        public CustomerRegistryDto BuildDto(ICustomerRegistry entity)
        {
            var dto = new CustomerRegistryDto()
            {
                Id = entity.Id,
                ConsumerKey = entity.ConsumerKey,
                RegistryTfKey = entity.CustomerRegistryTfKey,
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate
            };

            return dto;
        }
    }
}
