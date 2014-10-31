namespace Merchello.Web.Models.ContentEditing
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The shipment display.
    /// </summary>
    public class ShipmentDisplay
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the shipment number prefix.
        /// </summary>
        public string ShipmentNumberPrefix { get; set; }

        /// <summary>
        /// Gets or sets the shipment number.
        /// </summary>
        public int ShipmentNumber { get; set; }

        /// <summary>
        /// Gets or sets the shipment status.
        /// </summary>
        public ShipmentStatusDisplay ShipmentStatus { get; set; }

        /// <summary>
        /// Gets or sets the shipment status key.
        /// </summary>
        public Guid ShipmentStatusKey { get; set; }

        /// <summary>
        /// Gets or sets the shipped date.
        /// </summary>
        public DateTime ShippedDate { get; set; }

        /// <summary>
        /// Gets or sets the version key.
        /// </summary>
        public Guid VersionKey { get; set; }

        /// <summary>
        /// Gets or sets the from organization.
        /// </summary>
        public string FromOrganization { get; set; }

        /// <summary>
        /// Gets or sets the from name.
        /// </summary>
        public string FromName { get; set; }

        /// <summary>
        /// Gets or sets the from address 1.
        /// </summary>
        public string FromAddress1 { get; set; }

        /// <summary>
        /// Gets or sets the from address 2.
        /// </summary>
        public string FromAddress2 { get; set; }

        /// <summary>
        /// Gets or sets the from locality.
        /// </summary>
        public string FromLocality { get; set; }

        /// <summary>
        /// Gets or sets the from region.
        /// </summary>
        public string FromRegion { get; set; }

        /// <summary>
        /// Gets or sets the from postal code.
        /// </summary>
        public string FromPostalCode { get; set; }

        /// <summary>
        /// Gets or sets the from country code.
        /// </summary>
        public string FromCountryCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether from is commercial.
        /// </summary>
        public bool FromIsCommercial { get; set; }

        /// <summary>
        /// Gets or sets the to organization.
        /// </summary>
        public string ToOrganization { get; set; }

        /// <summary>
        /// Gets or sets the to name.
        /// </summary>
        public string ToName { get; set; }

        /// <summary>
        /// Gets or sets the to address 1.
        /// </summary>
        public string ToAddress1 { get; set; }

        /// <summary>
        /// Gets or sets the to address 2.
        /// </summary>
        public string ToAddress2 { get; set; }

        /// <summary>
        /// Gets or sets the to locality.
        /// </summary>
        public string ToLocality { get; set; }

        /// <summary>
        /// Gets or sets the to region.
        /// </summary>
        public string ToRegion { get; set; }

        /// <summary>
        /// Gets or sets the to postal code.
        /// </summary>
        public string ToPostalCode { get; set; }

        /// <summary>
        /// Gets or sets the to country code.
        /// </summary>
        public string ToCountryCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to is commercial.
        /// </summary>
        public bool ToIsCommercial { get; set; }

        /// <summary>
        /// Gets or sets the ship method key.
        /// </summary>
        public Guid? ShipMethodKey { get; set; }

        /// <summary>
        /// Gets or sets the phone.
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the carrier.
        /// </summary>
        public string Carrier { get; set; }

        /// <summary>
        /// Gets or sets the tracking code.
        /// </summary>
        public string TrackingCode { get; set; }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        public IEnumerable<OrderLineItemDisplay> Items { get; set; }
    }
}