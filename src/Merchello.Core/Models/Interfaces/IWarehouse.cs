using System;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a Merchello Warehouse object interface
    /// </summary>
    public interface IWarehouse : IEntity
    {
            
        /// <summary>
        /// The name for the Warehouse
        /// </summary>
        [DataMember]
        string Name { get; set;}
            
        /// <summary>
        /// The address1 for the Warehouse
        /// </summary>
        [DataMember]
        string Address1 { get; set;}
            
        /// <summary>
        /// The address2 for the Warehouse
        /// </summary>
        [DataMember]
        string Address2 { get; set;}
            
        /// <summary>
        /// The locality for the Warehouse
        /// </summary>
        [DataMember]
        string Locality { get; set;}
            
        /// <summary>
        /// The region for the Warehouse
        /// </summary>
        [DataMember]
        string Region { get; set;}
            
        /// <summary>
        /// The postalCode for the Warehouse
        /// </summary>
        [DataMember]
        string PostalCode { get; set;}

        /// <summary>
        /// The country code of the warehouse
        /// </summary>
        [DataMember]
        string CountryCode { get; set; }

        /// <summary>
        /// The phone number of the warehouse
        /// </summary>
        [DataMember]
        string Phone { get; set; }

        /// <summary>
        /// The contact email address of the warhouse
        /// </summary>
        [DataMember]
        string Email { get; set; }
    }
}



