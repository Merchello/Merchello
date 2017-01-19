namespace Merchello.Core.Strategies.Itemization
{
    using System.Collections.Generic;

    using Merchello.Core.Models;

    /// <summary>
    /// Represents a invoice itemization.
    /// </summary>
    public class InvoiceItemItemization
    {
        /// <summary>
        /// Gets or sets the collection of the product line items.
        /// </summary>
        public IEnumerable<ILineItem> Products { get; set; }

        /// <summary>
        /// Gets or sets the collection of shipping line items.
        /// </summary>
        public IEnumerable<ILineItem> Shipping { get; set; }

        /// <summary>
        /// Gets or sets the collection of tax line items.
        /// </summary>
        public IEnumerable<ILineItem> Tax { get; set; }

        /// <summary>
        /// Gets or sets the collection of adjustment line items.
        /// </summary>
        public IEnumerable<ILineItem> Adjustments { get; set; }

        /// <summary>
        /// Gets or sets the collection of discount line items.
        /// </summary>
        public IEnumerable<ILineItem> Discounts { get; set; }

        /// <summary>
        /// Gets or sets the collection of custom line items.
        /// </summary>
        public IEnumerable<ILineItem> Custom { get; set; }

        /// <summary>
        /// Gets a value indicating whether reconciles.
        /// </summary>
        public bool Reconciles { get; internal set; }

        /// <summary>
        /// Gets the shipping total.
        /// </summary>
        public decimal ShippingTotal { get; internal set; }

        /// <summary>
        /// Gets the tax total.
        /// </summary>
        public decimal TaxTotal { get; internal set; }

        /// <summary>
        /// Gets the adjustment total.
        /// </summary>
        public decimal AdjustmentTotal { get; internal set; }

        /// <summary>
        /// Gets the product total.
        /// </summary>
        public decimal ProductTotal { get; internal set; }

        /// <summary>
        /// Gets the discount total.
        /// </summary>
        public decimal DiscountTotal { get; internal set; }

        /// <summary>
        /// Gets the custom total.
        /// </summary>
        public decimal CustomTotal { get; internal set; }

        /// <summary>
        /// Gets the invoice total of the invoice.
        /// </summary>
        public decimal InvoiceTotal { get; internal set; }

        /// <summary>
        /// Gets the total of the invoice itemization.
        /// </summary>
        public decimal ItemizationTotal { get; internal set; }
    }
}