namespace Merchello.Core.Models
{
    /// <summary>
    /// Represents a standard address
    /// </summary>
    public interface IAddress : IShallowClone
    {
        /// <summary>
        /// Gets or sets the name for the address
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the first address line
        /// </summary>
        string Address1 { get; set; }

        /// <summary>
        /// Gets or sets the second address line
        /// </summary>
        string Address2 { get; set; }

        /// <summary>
        /// Gets or sets the city or locality of the address
        /// </summary>
        string Locality { get; set; }

        /// <summary>
        /// Gets or sets the state or province of the address
        /// </summary>
        string Region { get; set; }

        /// <summary>
        /// Gets or sets the postal code of the address
        /// </summary>
        string PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the country code of the address
        /// </summary>
        string CountryCode { get; set; }

        /// <summary>
        /// Gets or sets the telephone number of the address
        /// </summary>
        string Phone { get; set; }

        /// <summary>
        /// Gets or sets the email address associated with the address
        /// </summary>
        string Email { get; set; }

        /// <summary>
        /// Gets or sets the organization or company name associated with the address
        /// </summary>
        string Organization { get; set; }

        /// <summary>
        /// Gets or sets the address type.
        /// </summary>
        AddressType AddressType { get; set; }
    }
}