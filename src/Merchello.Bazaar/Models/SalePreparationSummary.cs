namespace Merchello.Bazaar.Models
{
    using System.Collections.Generic;

    using Merchello.Core.Models;

    /// <summary>
    /// The confirmation pre sale summary.
    /// </summary>
    public class SalePreparationSummary
    {
        /// <summary>
        /// Gets or sets the total label.
        /// </summary>
        public string TotalLabel { get; set; }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        public IEnumerable<BasketLineItem> Items { get; set; }

        /// <summary>
        /// Gets or sets the item total.
        /// </summary>
        public decimal ItemTotal { get; set; }

        /// <summary>
        /// Gets or sets the shipping total.
        /// </summary>
        public decimal ShippingTotal { get; set; }

        /// <summary>
        /// Gets or sets the tax total.
        /// </summary>
        public decimal TaxTotal { get; set; }

        /// <summary>
        /// Gets or sets the discounts total.
        /// </summary>
        public decimal DiscountsTotal { get; set; }

        /// <summary>
        /// Gets or sets the invoice total.
        /// </summary>
        public decimal InvoiceTotal { get; set; }

        /// <summary>
        /// Gets or sets the currency.
        /// </summary>
        public ICurrency Currency { get; set; }
    }
}