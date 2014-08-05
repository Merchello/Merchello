namespace Merchello.Core.Models
{
    using Services;

    /// <summary>
    /// Extension methods for IAddress
    /// </summary>
    public static class AddressExtensions
    {
        /// <summary>
        /// Gets the <see cref="ICountry"/> for the <see cref="IAddress"/>
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <returns>
        /// The <see cref="ICountry"/>.
        /// </returns>
        public static ICountry Country(this IAddress address)
        {
            if (string.IsNullOrEmpty(address.CountryCode)) return null;

            return new Country(address.CountryCode, StoreSettingService.GetProvincesByCountryCode(address.CountryCode))
                {
                    ProvinceLabel = StoreSettingService.GetProvinceLabelForCountry(address.CountryCode)
                };
        }

        /// <summary>
        /// Maps a <see cref="IAddress"/> to a <see cref="ICustomerAddress"/>
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="addressType">The type of address to be saved</param>
        /// <returns>
        /// The <see cref="ICustomerAddress"/>.
        /// </returns>
        internal static ICustomerAddress ToCustomerAddress(this IAddress address, ICustomer customer, AddressType addressType)
        {
            return new CustomerAddress(customer.Key)
            {
                Address1 = address.Address1,
                Address2 = address.Address2,
                Locality = address.Locality,
                Region = address.Region,
                PostalCode = address.PostalCode,
                CountryCode = address.CountryCode,
                Phone = address.Phone,                
                Label = string.IsNullOrEmpty(address.Name) ? "Address" : address.Name, 
                Company = address.Organization,
                AddressType = addressType
            };
        }
    }
}