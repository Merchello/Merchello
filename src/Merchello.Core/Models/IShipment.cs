using System;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a Merchello Shipment object interface
    /// </summary>
    public interface IShipment : IIdEntity
    {
            
            /// <summary>
            /// The invoiceId for the Shipment
            /// </summary>
            [DataMember]
            int InvoiceId { get; }
            
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
            /// The shipMethodId for the Shipment
            /// </summary>
            [DataMember]
            int ShipMethodId { get; }
            
            /// <summary>
            /// The phone number at the shipping address for the Shipment
            /// </summary>
            [DataMember]
            string Phone { get; set;}
    }
}



