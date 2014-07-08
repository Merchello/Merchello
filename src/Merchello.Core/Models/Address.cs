namespace Merchello.Core.Models
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents an address
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class Address : IAddress
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Address"/> class.
        /// </summary>
        public Address()
        {
            AddressType = AddressType.Shipping;
        }

        /// <summary>
        /// Gets or sets he name for the address
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the first address line
        /// </summary>
        [DataMember]
        public string Address1 { get; set; }

        /// <summary>
        /// Gets or sets second address line
        /// </summary>
        [DataMember]
        public string Address2 { get; set; }

        /// <summary>
        /// Gets or sets the city or locality of the address
        /// </summary>
        [DataMember]
        public string Locality { get; set; }

        /// <summary>
        /// Gets or sets the state or province of the address
        /// </summary>
        [DataMember]
        public string Region { get; set; }

        /// <summary>
        /// Gets or sets the postal code of the address
        /// </summary>
        [DataMember]
        public string PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the country code of the address
        /// </summary>
        [DataMember]
        public string CountryCode { get; set; }

        /// <summary>
        /// Gets or sets the telephone number of the address
        /// </summary>
        [DataMember]
        public string Phone { get; set; }

        /// <summary>
        /// Gets or sets the email address associated with the address
        /// </summary>
        [DataMember]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the organization or company name associated with the address
        /// </summary>
        [DataMember]
        public string Organization { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not this record represents commercial or business address
        /// </summary>
        /// <remarks>
        /// Used by certain shipping providers in shipping rate quotations
        /// </remarks>
        [DataMember]
        public bool IsCommercial { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="AddressType"/> of the address
        /// </summary>
        [DataMember]
        public AddressType AddressType { get; set; }

        /// <summary>
        /// Overrides the default equals comparison method
        /// </summary>
        /// <param name="other">
        /// The "other" address to compare to "this" address
        /// </param>
        /// <returns>
        /// A value indicating whether or not the addresses are to be considered equal
        /// </returns>
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