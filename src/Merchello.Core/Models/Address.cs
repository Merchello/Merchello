using System;
using System.Runtime.Serialization;
using Merchello.Core.Models.Interfaces;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Represents an address
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class Address : IAddress
    {
        public Address()
        {
            AddressType = AddressType.Shipping;
        }

        /// <summary>
        /// The name for the address
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// The first address line
        /// </summary>
        [DataMember]
        public string Address1 { get; set; }

        /// <summary>
        /// The second address line
        /// </summary>
        [DataMember]
        public string Address2 { get; set; }

        /// <summary>
        /// The city or locality of the address
        /// </summary>
        [DataMember]
        public string Locality { get; set; }

        /// <summary>
        /// The state or province of the address
        /// </summary>
        [DataMember]
        public string Region { get; set; }

        /// <summary>
        /// the postal code of the address
        /// </summary>
        [DataMember]
        public string PostalCode { get; set; }

        /// <summary>
        /// The country code of the address
        /// </summary>
        [DataMember]
        public string CountryCode { get; set; }

        /// <summary>
        /// The telephone number of the address
        /// </summary>
        [DataMember]
        public string Phone { get; set; }

        /// <summary>
        /// The email address associated with the address
        /// </summary>
        [DataMember]
        public string Email { get; set; }

        /// <summary>
        /// The organization or company name associated with the address
        /// </summary>
        [DataMember]
        public string Organization { get; set; }

        /// <summary>
        /// True/false indicating whether or not this record represents commercial or business address
        /// </summary>
        /// <remarks>
        /// Used by certain shipping providers in shipping rate quotations
        /// </remarks>
        [DataMember]
        public bool IsCommercial { get; set; }

        /// <summary>
        /// The <see cref="AddressType"/> of the address
        /// </summary>
        [DataMember]
        AddressType AddressType { get; set; }


        public virtual bool Equals(IAddress other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Name == other.Name &&
                   Address1 == other.Address1 &&
                   Address2 == other.Address2 &&
                   Locality == other.Locality &&
                   Region == other.Region &&
                   PostalCode == other.PostalCode &&
                   CountryCode == other.CountryCode &&
                   IsCommercial == other.IsCommercial;
        }
    }
}