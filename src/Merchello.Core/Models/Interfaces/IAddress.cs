namespace Merchello.Core.Models
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines a standard address
    /// </summary>
    public interface IAddress
    {
        /// <summary>
        /// Gets or sets the name for the address
        /// </summary>
        [DataMember]
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the first address line
        /// </summary>
        [DataMember]
        string Address1 { get; set; }

        /// <summary>
        /// Gets or sets the second address line
        /// </summary>
        [DataMember]
        string Address2 { get; set; }

        /// <summary>
        /// Gets or sets the city or locality of the address
        /// </summary>
        [DataMember]
        string Locality { get; set; }

        /// <summary>
        /// Gets or sets the state or province of the address
        /// </summary>
        [DataMember]
        string Region { get; set; }

        /// <summary>
        /// Gets or sets the postal code of the address
        /// </summary>
        [DataMember]
        string PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the country code of the address
        /// </summary>
        [DataMember]
        string CountryCode { get; set; }

        /// <summary>
        /// Gets or sets the telephone number of the address
        /// </summary>
        [DataMember]
        string Phone { get; set; }

        /// <summary>
        /// Gets or sets the email address associated with the address
        /// </summary>
        [DataMember]
        string Email { get; set; }

        /// <summary>
        /// Gets or sets the organization or company name associated with the address
        /// </summary>
        [DataMember]
        string Organization { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not this record represents commercial or business address
        /// </summary>
        /// <remarks>
        /// Used by certain shipping providers in shipping rate quotations
        /// </remarks>
        [DataMember]
        bool IsCommercial { get; set; }

        /// <summary>
        /// Gets or sets the address type.
        /// </summary>
        AddressType AddressType { get; set; }
    }
}