using System;
using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    internal class AddressFactory : IEntityFactory<IAddress, AddressDto>
    {

        public IAddress BuildEntity(AddressDto dto)
        {
            var address = new Address(dto.Id, dto.CustomerPk, dto.Label)
            {
                Id = dto.Id, 
                CustomerPk = dto.CustomerPk, 
                Label = dto.Label,
                CreateDate = dto.CreateDate,
                UpdateDate = dto.UpdateDate
            };

            address.ResetDirtyProperties();

            return address;
        }

        public AddressDto BuildDto(IAddress entity)
        {
            var dto = new AddressDto()
            {
                Id = entity.Id,
                CustomerPk = entity.CustomerPk,
                Label = entity.Label,
                FullName = entity.Label,
                Company = entity.Company,
                AddressTypeFieldKey = entity.AddressTypeFieldKey,
                Address1 = entity.Address1,
                Address2 = entity.Address2,
                Locality = entity.Locality,
                Region = entity.Region,
                PostalCode = entity.PostalCode,
                CountryCode = entity.CountryCode,
                Phone = entity.Phone,
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate
            };

            return dto;
        }
    }
}
