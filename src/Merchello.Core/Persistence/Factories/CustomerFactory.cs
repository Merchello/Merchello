using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                    Key = dto.Pk,
                    MemberId = dto.MemberId,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    CreateDate = dto.CreateDate,
                    UpdateDate = dto.UpdateDate
                };


            //TODO: set to ResetDirtyProperties(false)
            customer.ResetDirtyProperties();

            return customer;
        }

        public CustomerDto BuildDto(ICustomer entity)
        {
            var dto = new CustomerDto()
                {
                    Pk = entity.Key,
                    MemberId = entity.MemberId == 0 || entity.MemberId == null ? null : entity.MemberId,
                    FirstName = entity.FirstName,
                    LastName = entity.LastName,
                    TotalInvoiced = entity.TotalInvoiced,
                    TotalPayments = entity.TotalPayments,
                    LastPaymentDate = entity.LastPaymentDate,
                    UpdateDate = entity.UpdateDate,
                    CreateDate = entity.CreateDate
                };

            return dto;
        }
    }
}
