namespace Merchello.Plugin.Taxation.Avalara.Models.Address
{
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;

    using Merchello.Core.Models;
    using Merchello.Plugin.Taxation.Avalara.Models.Tax;

    /// <summary>
    /// Defines a TaxAddress used in tax requests.
    /// </summary>
    public interface ITaxAddress : IValidatableAddress
    {
        /// <summary>
        /// Gets or sets the address code.
        /// </summary>
        /// <remarks>
        /// This is basically an index reference to for the various addresses in a <see cref="TaxRequest"/>.
        /// </remarks>
        string AddressCode { get; set; }

        /// <summary>
        /// Gets or sets the latitude of the address (Optional)
        /// </summary>
        decimal? Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude of the address (Optional)
        /// </summary>
        decimal? Longitude { get; set; }

        /// <summary>
        /// Gets or sets the tax region id (Optional)
        /// </summary>
        int? TaxRegionId { get; set; }
    }
}