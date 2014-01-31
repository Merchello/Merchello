using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    internal class CustomerFactory : IEntityFactory<ICustomer, CustomerDto>
    {
        public ICustomer BuildEntity(CustomerDto dto)
        {
            var customer = new Customer(dto.TotalInvoiced, dto.TotalPayments, dto.LastPaymentDate)
                {
                    Key = dto.Key,
                    EntityKey = dto.EntityKey,
                    MemberId = dto.MemberId,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email,
                    ExtendedData = new ExtendedDataCollection(dto.ExtendedData),
                    CreateDate = dto.CreateDate,
                    UpdateDate = dto.UpdateDate
                };

            customer.ResetDirtyProperties();

            return customer;
        }

        public CustomerDto BuildDto(ICustomer entity)
        {
            var dto = new CustomerDto()
                {
                    Key = entity.Key,
                    MemberId = entity.MemberId == 0 || entity.MemberId == null ? null : entity.MemberId,
                    FirstName = entity.FirstName,
                    LastName = entity.LastName,
                    Email = entity.Email,
                    TotalInvoiced = entity.TotalInvoiced,
                    TotalPayments = entity.TotalPayments,
                    LastPaymentDate = entity.LastPaymentDate,
                    EntityKey = entity.EntityKey,
                    ExtendedData = entity.ExtendedData.SerializeToXml(),
                    UpdateDate = entity.UpdateDate,
                    CreateDate = entity.CreateDate
                };

            return dto;
        }       
        
    }
}
