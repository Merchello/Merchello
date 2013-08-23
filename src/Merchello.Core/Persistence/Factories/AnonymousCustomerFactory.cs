using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    internal class AnonymousCustomerFactory : IEntityFactory<IAnonymousCustomer, AnonymousDto>
    {
        public IAnonymousCustomer BuildEntity(AnonymousDto dto)
        {
            return new AnonymousCustomer(dto.LastActivityDate)
            {
                Key = dto.Key,
                UpdateDate = dto.UpdateDate,
                CreateDate = dto.CreateDate                
            };
        }

        public AnonymousDto BuildDto(IAnonymousCustomer entity)
        {
            return new AnonymousDto()
            {
                Key = entity.Key,
                LastActivityDate = entity.LastActivityDate,
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate
            };
        }
    }
}
