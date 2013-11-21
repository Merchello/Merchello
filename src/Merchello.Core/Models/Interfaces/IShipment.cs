using System;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a Merchello Shipment object interface
    /// </summary>
    public interface IShipment : IEntity
    {                        
        /// <summary>
        /// The line 1 of the shipping address for the Shipment
        /// </summary>
        [DataMember]
        string Address1 { get; set;}
            
        /// <summary>
        /// The line 2 of the shipping address for the Shipment
        /// </summary>
        [DataMember]
        string Address2 { get; set;}
            
        /// <summary>
        /// The locality or city of the shipping address for the Shipment
        /// </summary>
        [DataMember]
        string Locality { get; set;}
            
        /// <summary>
        /// The region, state or province of the shipping address for the Shipment
        /// </summary>
        [DataMember]
        string Region { get; set;}
            
        /// <summary>
        /// The postal or zip code of the shipping address for the Shipment
        /// </summary>
        [DataMember]
        string PostalCode { get; set;}
            
        /// <summary>
        /// The country code of the shipping address for the Shipment
        /// </summary>
        [DataMember]
        string CountryCode { get; set;}
            
        /// <summary>
        /// The ship method associated with this shipment
        /// </summary>
        /// <remarks>
        /// This is nullable in case a provider (and related shipmethods) is deleted and we want to maintain the shipment record
        /// </remarks>
        [DataMember]
        Guid? ShipMethodKey { get; set; }

        /// <summary>
        /// The invoice item, if any, associated with this shipment
        /// </summary>
        [DataMember]
        Guid? InvoiceItemKey { get; set; }
            
        /// <summary>
        /// The phone number at the shipping address for the Shipment
        /// </summary>
        [DataMember]
        string Phone { get; set;}
    }
}



