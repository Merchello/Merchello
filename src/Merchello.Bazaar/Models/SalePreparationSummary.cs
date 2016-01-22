using Merchello.Core.Models;

namespace Merchello.Bazaar.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The confirmation pre sale summary.
    /// </summary>
    public partial class SalePreparationSummary
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
        public string ItemTotal { get; set; }

        /// <summary>
        /// Gets or sets the shipping total.
        /// </summary>
        public string ShippingTotal { get; set; }

        /// <summary>
        /// Gets or sets the tax total.
        /// </summary>
        public string TaxTotal { get; set; }

        /// <summary>
        /// Gets or sets the discounts total.
        /// </summary>
        public string DiscountsTotal { get; set; }

        /// <summary>
        /// Gets or sets the invoice total.
        /// </summary>
        public string InvoiceTotal { get; set; }

        /// <summary>
        /// Gets or sets the currency symbol.
        /// </summary>
        public ICurrency Currency { get; set; }
    }
}