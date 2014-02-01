using System;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a Merchello Shipment object interface
    /// </summary>
    public interface IShipment : ILineItemContainer
    {

        /// <summary>
        /// The origin address's name for the Shipment
        /// </summary>
        [DataMember]
        string FromName { get; set; }

        /// <summary>
        /// The origin address line 1 for the Shipment
        /// </summary>
        [DataMember]
        string FromAddress1 { get; set; }

        /// <summary>
        /// The origin address line 2 for the Shipment
        /// </summary>
        [DataMember]
        string FromAddress2 { get; set; }

        /// <summary>
        /// The origin address locality or city for the Shipment
        /// </summary>
        [DataMember]
        string FromLocality { get; set; }

        /// <summary>
        /// The origin address region, state or province for the Shipment
        /// </summary>
        [DataMember]
        string FromRegion { get; set; }

        /// <summary>
        /// The origin address's postal code for the Shipment
        /// </summary>
        [DataMember]
        string FromPostalCode { get; set; }

        /// <summary>
        /// The origin address's country code for the Shipment
        /// </summary>
        [DataMember]
        string FromCountryCode { get; set; }

        /// <summary>
        /// True/false indicating whether or not the origin's address is a commercial address. Used by some shipping providers.
        /// </summary>
        [DataMember]
        bool FromIsCommercial { get; set; }

        /// <summary>
        /// The destination address's name or company for the Shipment
        /// </summary>
        [DataMember]
        string ToName { get; set; }

        /// <summary>
        /// The destination address line 1 for the Shipment
        /// </summary>
        [DataMember]
        string ToAddress1 { get; set;}
            
        /// <summary>
        /// The destination address line 2 for the Shipment
        /// </summary>
        [DataMember]
        string ToAddress2 { get; set;}
            
        /// <summary>
        /// The destination address locality or city for the Shipment
        /// </summary>
        [DataMember]
        string ToLocality { get; set;}
            
        /// <summary>
        /// The destination address region, state or province for the Shipment
        /// </summary>
        [DataMember]
        string ToRegion { get; set;}
            
        /// <summary>
        /// The destination address's postal code for the Shipment
        /// </summary>
        [DataMember]
        string ToPostalCode { get; set;}
            
        /// <summary>
        /// The destination address's country code for the Shipment
        /// </summary>
        [DataMember]
        string ToCountryCode { get; set;}

        /// <summary>
        /// True/false indicating whether or not the destination address is a commercial address.  Used by some shipping providers.
        /// </summary>
        [DataMember]
        bool ToIsCommercial { get; set; }
                        
        /// <summary>
        /// The ship method associated with this shipment
        /// </summary>
        /// <remarks>
        /// This is nullable in case a provider (and related shipmethods) is deleted and we want to maintain the shipment record
        /// </remarks>
        [DataMember]
        Guid? ShipMethodKey { get; set; }
            
        /// <summary>
        /// The phone number at the shipping address for the Shipment
        /// </summary>
        [DataMember]
        string Phone { get; set;}

        /// <summary>
        /// The contact email address associated with this shipment
        /// </summary>
        [DataMember]
        string Email { get; set; }

    }
}



