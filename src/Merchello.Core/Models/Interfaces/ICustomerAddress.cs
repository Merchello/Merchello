namespace Merchello.Core.Models
{
    using System;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Defines a Merchello customer
    /// </summary>
    public interface ICustomerAddress : IEntity
    {
        /// <summary>
        /// Gets the customer key.
        /// </summary>
        [DataMember]
        Guid CustomerKey { get; }

        /// <summary>
        /// Gets or sets the descriptive label for the address
        /// </summary>
        [DataMember]
        string Label { get; set; }

        /// <summary>
        /// Gets or sets the full name for the address
        /// </summary>
        [DataMember]
        string FullName { get; set; }

        /// <summary>
        /// Gets or sets company name for the address
        /// </summary>
        [DataMember]
        string Company { get; set; }

        /// <summary>
        /// Gets or sets type of address indicator
        /// </summary>
        [DataMember]
        Guid AddressTypeFieldKey { get; set; }

        /// <summary>
        /// Gets or sets first address line
        /// </summary>
        [DataMember]
        string Address1 { get; set; }

        /// <summary>
        /// Gets or sets second address line
        /// </summary>
        [DataMember]
        string Address2 { get; set; }

        /// <summary>
        /// Gets or sets the city or locality of the address
        /// </summary>
        [DataMember]
        string Locality { get; set; }

        /// <summary>
        /// Gets or sets state or province of the address
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
        /// Gets or sets the <see cref="AddressType"/> of the address
        /// </summary>
        [DataMember]
        AddressType AddressType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is default.
        /// </summary>
        [DataMember]
        bool IsDefault { get; set; }
    }
}
