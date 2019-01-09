namespace Merchello.Web.Models.ContentEditing
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Merchello.Core;
    using Merchello.Core.Configuration;
    using Merchello.Core.Models;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// The address display.
    /// </summary>
    public class AddressDisplay
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the address 1.
        /// </summary>
        public string Address1 { get; set; }

        /// <summary>
        /// Gets or sets the address 2.
        /// </summary>
        public string Address2 { get; set; }

        /// <summary>
        /// Gets or sets the locality.
        /// </summary>
        public string Locality { get; set; }

        /// <summary>
        /// Gets or sets the region.
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// Gets or sets the postal code.
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the country code.
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// Gets or sets the country name
        /// </summary>
        public string CountryName { get; set; }

        /// <summary>
        /// Gets or sets the phone.
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the organization.
        /// </summary>
        public string Organization { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the address is a commercial address.
        /// </summary>
        public bool IsCommercial { get; set; }

        /// <summary>
        /// Gets or sets the address type.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public AddressType AddressType { get; set; }
    }

    /// <summary>
    /// Mapping extensions for <see cref="AddressDisplay"/>
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    internal static class AddressDetailsMappingExtensions
    {
        /// <summary>
        /// Maps <see cref="IAddress"/> to <see cref="AddressDisplay"/>
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <returns>
        /// The <see cref="AddressDisplay"/>.
        /// </returns>
        internal static AddressDisplay ToAddressDisplay(this IAddress address)
        {
            var mappedAddress = AutoMapper.Mapper.Map<AddressDisplay>(address);
            var country = MerchelloConfiguration.Current.MerchelloCountries().Countries.FirstOrDefault(x => x.CountryCode.Equals(mappedAddress.CountryCode, StringComparison.InvariantCultureIgnoreCase));
            if (country != null)
            {
                mappedAddress.CountryName = country.Name;
            }
            return mappedAddress;
        }

        /// <summary>
        /// Maps <see cref="AddressDisplay"/> to <see cref="IAddress"/>
        /// </summary>
        /// <param name="addressDisplay">
        /// The address display.
        /// </param>
        /// <returns>
        /// The <see cref="IAddress"/>.
        /// </returns>
        internal static IAddress ToAddress(this AddressDisplay addressDisplay)
        {
            return AutoMapper.Mapper.Map<Address>(addressDisplay);
        }
    }
}