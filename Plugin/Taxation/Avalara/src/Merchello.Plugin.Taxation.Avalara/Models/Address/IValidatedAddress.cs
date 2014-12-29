namespace Merchello.Plugin.Taxation.Avalara.Models.Address
{
    /// <summary>
    /// Defines a Validated Address.
    /// </summary>
    /// <remarks>
    /// This is the model that is returned from the Validate API
    /// </remarks>
    public interface IValidatedAddress : IValidatableAddress
    {
        /// <summary>
        /// Gets or sets the county.
        /// </summary>
        string County { get; set; }

        /// <summary>
        /// Gets or sets the FIPS code.
        /// </summary>
        string FipsCode { get; set; }

        /// <summary>
        /// Gets or sets the carrier route.
        /// </summary>
        string CarrierRoute { get; set; }

        /// <summary>
        /// Gets or sets the post net.
        /// </summary>
        string PostNet { get; set; }

        /// <summary>
        /// Gets or sets the address type.
        /// </summary>
        AvaTaxAddressType? AddressType { get; set; } 
    }
}