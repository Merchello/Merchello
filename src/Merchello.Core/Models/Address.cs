namespace Merchello.Core.Models
{
    using System;
    using System.Runtime.Serialization;

    /// <inheritdoc/>
    [Serializable]
    [DataContract(IsReference = true)]
    public class Address : IAddress
    {
        /// <inheritdoc/>
        public Address()
        {
            AddressType = AddressType.Shipping;
        }

        /// <inheritdoc/>
        [DataMember]
        public string Name { get; set; }

        /// <inheritdoc/>
        [DataMember]
        public string Address1 { get; set; }

        /// <inheritdoc/>
        [DataMember]
        public string Address2 { get; set; }

        /// <inheritdoc/>
        [DataMember]
        public string Locality { get; set; }

        /// <summary>
        /// Gets or sets the state or province of the address
        /// </summary>
        [DataMember]
        public string Region { get; set; }

        /// <inheritdoc/>
        [DataMember]
        public string PostalCode { get; set; }

        /// <inheritdoc/>
        [DataMember]
        public string CountryCode { get; set; }

        /// <inheritdoc/>
        [DataMember]
        public string Phone { get; set; }

        /// <inheritdoc/>
        [DataMember]
        public string Email { get; set; }

        /// <inheritdoc/>
        [DataMember]
        public string Organization { get; set; }

        /// <inheritdoc/>
        [DataMember]
        public bool IsCommercial { get; set; }

        /// <inheritdoc/>
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