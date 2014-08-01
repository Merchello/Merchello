namespace Merchello.Plugin.Taxation.Avalara.Models.Address
{
    /// <summary>
    /// Represents a verifiable address.
    /// </summary>
    public class ValidatableAddress : IValidatableAddress
    {
        /// <summary>
        /// Gets or sets the address line 1. Required
        /// </summary>
        public string Line1 { get; set; }

        /// <summary>
        /// Gets or sets the address line 2. Optional
        /// </summary>
        public string Line2 { get; set; }

        /// <summary>
        /// Gets or sets the address line 3. Optional
        /// </summary>
        public string Line3 { get; set; }

        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the region, state or province
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// Gets or sets the postal code.
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the country code
        /// </summary>
        public string Country { get; set; }
    }
}