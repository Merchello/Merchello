namespace Merchello.Plugin.Taxation.Avalara.Services
{
    using Merchello.Plugin.Taxation.Avalara.Models;
    using Merchello.Plugin.Taxation.Avalara.Models.Address;
    using Merchello.Plugin.Taxation.Avalara.Models.Tax;

    /// <summary>
    /// Defines an Avalara AvaTax Service.
    /// </summary>
    public interface IAvaTaxService
    {
        /// <summary>
        /// Validates an address against the Avalara AvaTax API.
        /// </summary>
        /// <param name="address">
        /// A validatable address.
        /// </param>
        /// <returns>
        /// The <see cref="IAddressValidationResult"/>.
        /// </returns>
        AddressValidationResult ValidateTaxAddress(IValidatableAddress address);

        /// <summary>
        /// Gets the <see cref="TaxResult"/> from the AvaTax API based on request parameters.
        /// </summary>
        /// <param name="request">
        /// The <see cref="TaxRequest"/>.
        /// </param>
        /// <returns>
        /// The <see cref="TaxResult"/>.
        /// </returns>
        TaxResult GetTax(TaxRequest request);

        /// <summary>
        /// Estimates taxes based on geo data and sales amount.
        /// </summary>
        /// <param name="latitude">
        /// The latitude.
        /// </param>
        /// <param name="longitude">
        /// The longitude.
        /// </param>
        /// <param name="saleAmount">
        /// The sale amount.
        /// </param>
        /// <returns>
        /// The <see cref="GeoTaxResult"/>.
        /// </returns>
        GeoTaxResult EstimateTax(decimal latitude, decimal longitude, decimal saleAmount);

        /// <summary>
        /// Pings an estimate tax call based on known latitude and longitude.
        /// </summary>
        /// <returns>
        /// The <see cref="GeoTaxResult"/>.
        /// </returns>
        /// <remarks>
        /// Uses US IRS Ogden UT address =)
        /// </remarks>
        GeoTaxResult Ping();

    }
}