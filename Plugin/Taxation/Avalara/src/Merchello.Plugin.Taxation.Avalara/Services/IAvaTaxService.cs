namespace Merchello.Plugin.Taxation.Avalara.Services
{
    using Merchello.Plugin.Taxation.Avalara.Models;
    using Merchello.Plugin.Taxation.Avalara.Models.Address;

    /// <summary>
    /// Defines an Avalara AvaTax Service.
    /// </summary>
    internal interface IAvaTaxService
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
    }
}