namespace Merchello.Web.Models.Shipping
{
    using System;

    using Merchello.Web.Models.ContentEditing;

    /// <summary>
    /// API object for handling new shipment requests.
    /// </summary>
    public class ShipmentRequestDisplay
    {
        /// <summary>
        /// Gets or sets the shipment status key.
        /// </summary>
        public Guid ShipmentStatusKey { get; set; }

        /// <summary>
        /// Gets or sets the ship method key.
        /// </summary>
        public Guid ShipMethodKey { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="OrderDisplay"/>.
        /// </summary>
        public OrderDisplay Order { get; set; }

        /// <summary>
        /// Gets or sets the carrier.
        /// </summary>
        public string Carrier { get; set; }

        /// <summary>
        /// Gets or sets the tracking number.
        /// </summary>
        public string TrackingNumber { get; set; }

        /// <summary>
        /// Gets or sets the tracking url.
        /// </summary>
        public string TrackingUrl { get; set; }
    }
}