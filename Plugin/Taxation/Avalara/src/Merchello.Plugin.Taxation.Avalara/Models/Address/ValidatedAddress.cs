namespace Merchello.Plugin.Taxation.Avalara.Models.Address
{
    /// <summary>
    /// Represents a validated address.
    /// </summary>
    public class ValidatedAddress : ValidatableAddress, IValidatedAddress
    {
        /// <summary>
        /// Gets or sets the county.
        /// </summary>
        public string County { get; set; }

        /// <summary>
        /// Gets or sets the FIPS code.
        /// </summary>
        public string FipsCode { get; set; }

        /// <summary>
        /// Gets or sets the postal carrier route.
        /// </summary>
        public string CarrierRoute { get; set; }

        /// <summary>
        /// Gets or sets the post net.
        /// </summary>
        public string PostNet { get; set; }

        /// <summary>
        /// Gets or sets the AvaTax address type.
        /// </summary>
        public AvaTaxAddressType? AddressType { get; set; }
    }
}