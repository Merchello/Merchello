namespace Merchello.Web.Store.Models
{
    using System;
    using System.Collections.Generic;

    using Merchello.Web.Models.Ui;

    /// <summary>
    /// Represents a checkout summary.
    /// </summary>
    public class StoreSummaryModel : ICheckoutSummaryModel<StoreAddressModel, StoreAddressModel, StoreLineItemModel>
    {
        /// <summary>
        /// Gets or sets the invoice key.
        /// </summary>
        /// <remarks>
        /// Used in receipts
        /// </remarks>
        public Guid InvoiceKey { get; set; }

        /// <summary>
        /// Gets or sets the invoice number.
        /// </summary>
        /// <remarks>
        /// Used in receipts
        /// </remarks>
        public string InvoiceNumber { get; set; }

        /// <summary>
        /// Gets or sets the invoice date.
        /// </summary>
        /// <remarks>
        /// Used in receipts
        /// </remarks>
        public DateTime InvoiceDate { get; set; }

        /// <summary>
        /// Gets or sets the billing address.
        /// </summary>
        public StoreAddressModel BillingAddress { get; set; }

        /// <summary>
        /// Gets or sets the shipping address.
        /// </summary>
        public StoreAddressModel ShippingAddress { get; set; }

        /// <summary>
        /// Gets or sets the checkout summary items.
        /// </summary>
        public IEnumerable<StoreLineItemModel> Items { get; set; }

        /// <summary>
        /// Gets or sets the sub total.
        /// </summary>
        public decimal Total { get; set; }
    }
}