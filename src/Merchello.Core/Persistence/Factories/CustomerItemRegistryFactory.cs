using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    internal class CustomerItemRegistryFactory : IEntityFactory<ICustomerItemRegister, CustomerItemRegisterDto>
    {
        public ICustomerItemRegister BuildEntity(CustomerItemRegisterDto dto)
        {
            var customerRegistry = new CustomerItemRegister(dto.ConsumerKey, dto.RegisterTfKey)
            {
                Id = dto.Id,
                UpdateDate = dto.UpdateDate,
                CreateDate = dto.CreateDate
            };

            customerRegistry.ResetDirtyProperties();

            return customerRegistry;
        }

        public CustomerItemRegisterDto BuildDto(ICustomerItemRegister entity)
        {
            var dto = new CustomerItemRegisterDto()
            {
                Id = entity.Id,
                ConsumerKey = entity.ConsumerKey,
                RegisterTfKey = entity.RegisterTfKey,
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate
            };

            return dto;
        }
    }
}
