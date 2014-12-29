namespace Merchello.Plugin.Taxation.Avalara.Models.Address
{
    using System.Diagnostics.CodeAnalysis;

    using Merchello.Core.Models;
    using Merchello.Plugin.Taxation.Avalara.Models.Tax;

    /// <summary>
    /// Represents a tax address.
    /// </summary>
    public class TaxAddress : ValidatableAddress, ITaxAddress
    {
        /// <summary>
        /// Gets or sets the address code.
        /// </summary>
        /// <remarks>
        /// This is basically an index reference to for the various addresses in a <see cref="TaxRequest"/>
        /// </remarks>
        public string AddressCode { get; set; }

        /// <summary>
        /// Gets or sets the latitude of the address (Optional)
        /// </summary>
        public decimal? Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude of the address (Optional)
        /// </summary>
        public decimal? Longitude { get; set; }

        /// <summary>
        /// Gets or sets the tax region id (Optional)
        /// </summary>
        /// <remarks>
        /// AvaTax tax region identifier. If a non-zero value is entered into TaxRegionId, other fields will be ignored.
        /// </remarks>
        public int? TaxRegionId { get; set; }
    }
}