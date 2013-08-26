using System;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a Merchello customer
    /// </summary>
    public interface IAddress : IIdEntity
    {

        /// <summary>
        /// The Customer primary key Guid
        /// </summary>
        [DataMember]
        Guid CustomerKey { get; }

        /// <summary>
        /// The descriptive label for the address
        /// </summary>
        [DataMember]
        string Label { get; set; }

        /// <summary>
        /// The full name for the address
        /// </summary>
        [DataMember]
        string FullName { get; set; }

        /// <summary>
        /// The company name for the address
        /// </summary>
        [DataMember]
        string Company { get; set; }

        /// <summary>
        /// The type of address indicator
        /// </summary>
        [DataMember]
        Guid AddressTypeFieldKey { get; set; }

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
        /// The <see cref="AddressType"/> of the address
        /// </summary>
        [DataMember]
        AddressType AddressType { get; set; }

    }
}
