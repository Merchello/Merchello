using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    internal class AnonymousCustomerFactory : IEntityFactory<IAnonymousCustomer, AnonymousCustomerDto>
    {
        public IAnonymousCustomer BuildEntity(AnonymousCustomerDto dto)
        {
            return new AnonymousCustomer()
            {
                Key = dto.Key,
                LastActivityDate = dto.LastActivityDate,
                UpdateDate = dto.UpdateDate,
                CreateDate = dto.CreateDate                
            };
        }

        public AnonymousCustomerDto BuildDto(IAnonymousCustomer entity)
        {
            return new AnonymousCustomerDto()
            {
                Key = entity.Key,
                LastActivityDate = entity.LastActivityDate,
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate
            };
        }
    }
}
