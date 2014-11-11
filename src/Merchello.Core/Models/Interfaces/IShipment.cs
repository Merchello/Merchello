namespace Merchello.Core.Models
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines a Merchello Shipment object interface
    /// </summary>
    public interface IShipment : ILineItemContainer
    {
        /// <summary>
        /// Gets or sets the shipment number prefix.
        /// </summary>
        string ShipmentNumberPrefix { get; set; }

        /// <summary>
        /// Gets or sets the shipment number.
        /// </summary>
        int ShipmentNumber { get; set; }

        /// <summary>
        /// Gets the shipment status key.
        /// </summary>
        Guid ShipmentStatusKey { get; }


        /// <summary>
        /// Gets or sets the Shipment Status
        /// </summary>
        [DataMember]
        IShipmentStatus ShipmentStatus { get; set; }

        /// <summary>
        /// Gets or sets the date the shipment was shipped
        /// </summary>
        [DataMember]
        DateTime ShippedDate { get; set; }

        /// <summary>
        /// Gets or sets the organization or company name associated with the address
        /// </summary>
        [DataMember]
        string FromOrganization { get; set; }

        /// <summary>
        /// Gets or sets the origin address's name for the Shipment
        /// </summary>
        [DataMember]
        string FromName { get; set; }

        /// <summary>
        /// Gets or sets the origin address line 1 for the Shipment
        /// </summary>
        [DataMember]
        string FromAddress1 { get; set; }

        /// <summary>
        /// Gets or sets the origin address line 2 for the Shipment
        /// </summary>
        [DataMember]
        string FromAddress2 { get; set; }

        /// <summary>
        /// Gets or sets the origin address locality or city for the Shipment
        /// </summary>
        [DataMember]
        string FromLocality { get; set; }

        /// <summary>
        /// Gets or sets the origin address region, state or province for the Shipment
        /// </summary>
        [DataMember]
        string FromRegion { get; set; }

        /// <summary>
        /// Gets or sets the origin address's postal code for the Shipment
        /// </summary>
        [DataMember]
        string FromPostalCode { get; set; }

        /// <summary>
        /// Gets or sets the origin address's country code for the Shipment
        /// </summary>
        [DataMember]
        string FromCountryCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the origin's address is a commercial address. Used by some shipping providers.
        /// </summary>
        [DataMember]
        bool FromIsCommercial { get; set; }

        /// <summary>
        /// Gets or sets the organization or company name associated with the address
        /// </summary>
        [DataMember]
        string ToOrganization { get; set; }

        /// <summary>
        /// Gets or sets the destination address's name or company for the Shipment
        /// </summary>
        [DataMember]
        string ToName { get; set; }

        /// <summary>
        /// Gets or sets the destination address line 1 for the Shipment
        /// </summary>
        [DataMember]
        string ToAddress1 { get; set; }
            
        /// <summary>
        /// Gets or sets the destination address line 2 for the Shipment
        /// </summary>
        [DataMember]
        string ToAddress2 { get; set; }
            
        /// <summary>
        /// Gets or sets the destination address locality or city for the Shipment
        /// </summary>
        [DataMember]
        string ToLocality { get; set; }
            
        /// <summary>
        /// Gets or sets the destination address region, state or province for the Shipment
        /// </summary>
        [DataMember]
        string ToRegion { get; set; }
            
        /// <summary>
        /// Gets or sets the destination address's postal code for the Shipment
        /// </summary>
        [DataMember]
        string ToPostalCode { get; set; }
            
        /// <summary>
        /// Gets or sets the destination address's country code for the Shipment
        /// </summary>
        [DataMember]
        string ToCountryCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the destination address is a commercial address.  Used by some shipping providers.
        /// </summary>
        [DataMember]
        bool ToIsCommercial { get; set; }
                        
        /// <summary>
        /// Gets or sets the ship method associated with this shipment
        /// </summary>
        /// <remarks>
        /// This is nullable in case a provider (and related shipmethods) is deleted and we want to maintain the shipment record
        /// </remarks>
        [DataMember]
        Guid? ShipMethodKey { get; set; }
            
        /// <summary>
        /// Gets or sets the phone number at the shipping address for the Shipment
        /// </summary>
        [DataMember]
        string Phone { get; set; }

        /// <summary>
        /// Gets or sets the contact email address associated with this shipment
        /// </summary>
        [DataMember]
        string Email { get; set; }

        /// <summary>
        /// Gets or sets the name of the freight carrier associated with this shipment
        /// </summary>
        [DataMember]
        string Carrier { get; set; }

        /// <summary>
        /// Gets or sets the tracking code associated with this shipment
        /// </summary>
        [DataMember]
        string TrackingCode { get; set; }
    }
}



