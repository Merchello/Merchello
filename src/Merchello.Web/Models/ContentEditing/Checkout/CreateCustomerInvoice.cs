namespace Merchello.Web.Models.ContentEditing.Checkout
{
    using System;

    /// <summary>
    /// A model for creating a customer invoice.
    /// </summary>
    public class CreateCustomerInvoice
    {
        /// <summary>
        /// Gets or sets the customer key.
        /// </summary>
        public Guid CustomerKey { get; set; }

        /// <summary>
        /// Gets or sets the customer billing address key.
        /// </summary>
        public Guid BillingAddressKey { get; set; }

        /// <summary>
        /// Gets or sets the customer shipping address key.
        /// </summary>
        public Guid ShippingAddressKey { get; set; }

        /// <summary>
        /// Gets or sets the ship method key.
        /// </summary>
        public Guid ShipMethodKey { get; set; } 
    }
}