namespace Merchello.Core.Persistence.Factories
{
    using Merchello.Core.Models;
    using Merchello.Core.Models.Rdbms;

    /// <summary>
    /// The customer address factory.
    /// </summary>
    internal class CustomerAddressFactory : IEntityFactory<ICustomerAddress, CustomerAddressDto>
    {
        /// <summary>
        /// The build entity.
        /// </summary>
        /// <param name="dto">
        /// The dto.
        /// </param>
        /// <returns>
        /// The <see cref="ICustomerAddress"/>.
        /// </returns>
        public ICustomerAddress BuildEntity(CustomerAddressDto dto)
        {
            var address = new CustomerAddress(dto.CustomerKey)
            {
                Key = dto.Key,
                Label = dto.Label,
                FullName = dto.FullName,
                Company = dto.Company,
                AddressTypeFieldKey = dto.AddressTfKey,
                Address1 = dto.Address1,
                Address2 = dto.Address2,
                Locality = dto.Locality,
                Region = dto.Region,
                PostalCode = dto.PostalCode,
                CountryCode = dto.CountryCode,
                Phone = dto.Phone,
                IsDefault = dto.IsDefault,
                CreateDate = dto.CreateDate,
                UpdateDate = dto.UpdateDate
            };

            address.ResetDirtyProperties();

            return address;
        }

        /// <summary>
        /// The build dto.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="CustomerAddressDto"/>.
        /// </returns>
        public CustomerAddressDto BuildDto(ICustomerAddress entity)
        {
            var dto = new CustomerAddressDto()
            {
                Key = entity.Key,
                CustomerKey = entity.CustomerKey,
                Label = entity.Label,
                FullName = entity.FullName,
                Company = entity.Company,
                AddressTfKey = entity.AddressTypeFieldKey,
                Address1 = entity.Address1,
                Address2 = entity.Address2,
                Locality = entity.Locality,
                Region = entity.Region,
                PostalCode = entity.PostalCode,
                CountryCode = entity.CountryCode,
                Phone = entity.Phone,
                IsDefault = entity.IsDefault,
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate
            };

            return dto;
        }
    }
}
