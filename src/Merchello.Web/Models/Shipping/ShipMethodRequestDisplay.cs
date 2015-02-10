namespace Merchello.Web.Models.Shipping
{
    using System;

    /// <summary>
    /// The ship method request display.
    /// </summary>
    public class ShipMethodRequestDisplay
    {
        /// <summary>
        /// Gets or sets the invoice key.
        /// </summary>
        public Guid InvoiceKey { get; set; }

        /// <summary>
        /// Gets or sets the line item key for the line item that contains the shipment
        /// </summary>
        public Guid LineItemKey { get; set; }

        /// <summary>
        /// Gets or sets the ship method key.
        /// </summary>
        public Guid ShipMethodKey { get; set; }
    }
}