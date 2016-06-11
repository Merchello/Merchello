namespace Merchello.Core.Models
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines a standard address
    /// </summary>
    public interface IAddress
    {
        /// <summary>
        /// The name for the address
        /// </summary>
        [DataMember]
        string Name { get; set; }

        /// <summary>
        /// The first address line
        /// </summary>
        [DataMember]
        string Address1 { get; set; }

        /// <summary>
        /// The second address line
        /// </summary>
        [DataMember]
        string Address2 { get; set; }

        /// <summary>
        /// The city or locality of the address
        /// </summary>
        [DataMember]
        string Locality { get; set; }

        /// <summary>
        /// The state or province of the address
        /// </summary>
        [DataMember]
        string Region { get; set; }

        /// <summary>
        /// the postal code of the address
        /// </summary>
        [DataMember]
        string PostalCode { get; set; }

        /// <summary>
        /// The country code of the address
        /// </summary>
        [DataMember]
        string CountryCode { get; set; }

        /// <summary>
        /// The telephone number of the address
        /// </summary>
        [DataMember]
        string Phone { get; set; }

        /// <summary>
        /// The email address associated with the address
        /// </summary>
        [DataMember]
        string Email { get; set; }

        /// <summary>
        /// The organization or company name associated with the address
        /// </summary>
        [DataMember]
        string Organization { get; set; }

        /// <summary>
        /// True/false indicating whether or not this record represents commercial or business address
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