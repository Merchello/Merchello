namespace Merchello.Web.Models.Reports
{
    using System.Collections.Generic;

    using Merchello.Web.Models.ContentEditing;

    /// <summary>
    /// The sales by item result.
    /// </summary>
    public class SalesByItemResult
    {
        /// <summary>
        /// Gets or sets the product variant.
        /// </summary>
        public ProductVariantDisplay ProductVariant { get; set; }

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the totals.
        /// </summary>
        public IEnumerable<ResultCurrencyValue> Totals { get; set; } 
    }
}