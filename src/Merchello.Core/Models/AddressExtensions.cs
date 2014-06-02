using Merchello.Core.Services;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Extension methods for IAddress
    /// </summary>
    public static class AddressExtensions
    {
        /// <summary>
        /// Gets the <see cref="ICountry"/> for the <see cref="IAddress"/>
        /// </summary>
        public static ICountry Country(this IAddress address)
        {
            if (string.IsNullOrEmpty(address.CountryCode)) return null;

            return new Country(address.CountryCode, StoreSettingService.GetProvincesByCountryCode(address.CountryCode))
                {
                    ProvinceLabel = StoreSettingService.GetProvinceLabelForCountry(address.CountryCode)
                };
        }
    }
}