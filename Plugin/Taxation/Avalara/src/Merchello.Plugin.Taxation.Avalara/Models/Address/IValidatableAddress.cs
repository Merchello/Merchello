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
}