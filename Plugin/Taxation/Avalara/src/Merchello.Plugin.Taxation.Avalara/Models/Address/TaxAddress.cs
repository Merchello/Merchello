namespace Merchello.Plugin.Taxation.Avalara.Models.Address
{
    /// <summary>
    /// Represents a tax address.
    /// </summary>
    public class TaxAddress : ValidatableAddress, ITaxAddress
    {
        /// <summary>
        /// Gets or sets the latitude of the address (Optional)
        /// </summary>
        public decimal Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude of the address (Optional)
        /// </summary>
        public decimal Longitude { get; set; }

        /// <summary>
        /// Gets or sets the tax region id (Optional)
        /// </summary>
        /// <remarks>
        /// AvaTax tax region identifier. If a non-zero value is entered into TaxRegionId, other fields will be ignored.
        /// </remarks>
        public int TaxRegionId { get; set; }
    }
}