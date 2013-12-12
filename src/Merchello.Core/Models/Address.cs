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
    internal class Address : IAddress
    {
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
    }
}