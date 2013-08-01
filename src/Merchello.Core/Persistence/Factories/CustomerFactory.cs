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

            return customer;
        }

        public CustomerDto BuildDto(ICustomer entity)
        {
            throw new NotImplementedException();
        }
    }
}
