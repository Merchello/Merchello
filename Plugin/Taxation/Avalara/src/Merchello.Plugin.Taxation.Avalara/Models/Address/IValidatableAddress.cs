namespace Merchello.Plugin.Taxation.Avalara.Models.Address
{
    using System.Diagnostics.CodeAnalysis;
    using System.Web;

    using Merchello.Core.Models;

    /// <summary>
    /// Defines an address that can be validated against the Avalara API.
    /// </summary>
    public interface IValidatableAddress
    {
        /// <summary>
        /// Gets or sets the address line 1. Required
        /// </summary>
        string Line1 { get; set; }

        /// <summary>
        /// Gets or sets the address line 2. Optional
        /// </summary>
        string Line2 { get; set; }

        /// <summary>
        /// Gets or sets the address line 3. Optional
        /// </summary>
        string Line3 { get; set; }

        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        string City { get; set; }

        /// <summary>
        /// Gets or sets the region, state or province
        /// </summary>
        string Region { get; set; }

        /// <summary>
        /// Gets or sets the postal code.
        /// </summary>
        string PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the country code
        /// </summary>
        string Country { get; set; }
    }

    #region Utility Extensions

    /// <summary>
    /// Extension methods to assist in IValidatableAddress operations.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    public static class ValidatableAddressExtensions
    {
        /// <summary>
        /// Maps a <see cref="IValidatableAddress"/> to an API usable query string.
        /// </summary>
        /// <param name="taxAddress">
        /// The tax address.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string AsApiQueryString(this IValidatableAddress taxAddress)
        {
            return string.Format(
                "{0}&{1}&{2}&{3}&{4}&{5}&{6}",
                GetQsValue("Line1", taxAddress.Line1),
                GetQsValue("Line2", taxAddress.Line2),
                GetQsValue("Line3", taxAddress.Line3),
                GetQsValue("City", taxAddress.City),
                GetQsValue("Region", taxAddress.Region),
                GetQsValue("PostalCode", taxAddress.PostalCode),
                GetQsValue("Country", taxAddress.Country));
        }

        /// <summary>
        /// Maps a <see cref="IAddress"/> to a <see cref="IValidatableAddress"/>.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <returns>
        /// The <see cref="IValidatableAddress"/>.
        /// </returns>
        public static IValidatableAddress ToValidatableAddress(this IAddress address)
        {
            return new ValidatableAddress()
                       {
                           Line1 = address.Address1,
                           Line2 = address.Address2,
                           City = address.Locality,
                           Region = address.Region,
                           PostalCode = address.PostalCode,
                           Country = address.CountryCode
                       };
        }

        /// <summary>
        /// Maps a <see cref="IValidatableAddress"/> to an <see cref="IAddress"/>.
        /// </summary>
        /// <param name="validatableAddress">
        /// The verifiable address.
        /// </param>
        /// <returns>
        /// The <see cref="IAddress"/>.
        /// </returns>
        public static IAddress ToAddress(this IValidatableAddress validatableAddress)
        {
            return new Address()
                       {
                           Address1 = validatableAddress.Line1,
                           Address2 = string.Format("{0} {1}", validatableAddress.Line2, validatableAddress.Line3).Trim(),
                           Locality = validatableAddress.City,
                           Region = validatableAddress.Region,
                           PostalCode = validatableAddress.PostalCode,
                           CountryCode = validatableAddress.Country
                       };
        }

        /// <summary>
        /// Utility method to safely encode query string parameters.
        /// </summary>
        /// <param name="prop">
        /// The prop.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetQsValue(string prop, string value)
        {
            return string.Format("{0}={1}", prop, HttpUtility.UrlEncode(value));
        }
    }

#endregion
}