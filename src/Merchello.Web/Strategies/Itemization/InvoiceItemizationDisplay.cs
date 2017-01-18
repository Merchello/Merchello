namespace Merchello.Web.Strategies.Itemization
{
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Web.Models.ContentEditing;

    /// <summary>
    /// Represents a invoice itemization.
    /// </summary>
    public class InvoiceItemizationDisplay
    {
        /// <summary>
        /// Gets or sets the collection of the product line items.
        /// </summary>
        public IEnumerable<InvoiceLineItemDisplay> Products { get; set; }

        /// <summary>
        /// Gets or sets the collection of shipping line items.
        /// </summary>
        public IEnumerable<InvoiceLineItemDisplay> Shipping { get; set; }

        /// <summary>
        /// Gets or sets the collection of tax line items.
        /// </summary>
        public IEnumerable<InvoiceLineItemDisplay> Tax { get; set; }

        /// <summary>
        /// Gets or sets the collection of adjustment line items.
        /// </summary>
        public IEnumerable<InvoiceLineItemDisplay> Adjustments { get; set; }

        /// <summary>
        /// Gets or sets the collection of discount line items.
        /// </summary>
        public IEnumerable<InvoiceLineItemDisplay> Discounts { get; set; }

        /// <summary>
        /// Gets or sets the collection of custom line items.
        /// </summary>
        public IEnumerable<InvoiceLineItemDisplay> Custom { get; set; }

        /// <summary>
        /// Gets a value indicating whether reconciles.
        /// </summary>
        public bool Reconciles { get; internal set; }

        /// <summary>
        /// Gets the total of the invoice.
        /// </summary>
        public decimal Total { get; internal set; }
    }


    internal static class InvoiceItemizationDisplayExtensions
    {
        public static decimal CalculateTotal(this InvoiceItemizationDisplay itemization)
        {
            var productTotal = itemization.Products.Sum(x => x.Price * x.Quantity);
            var shippingTotal = itemization.Shipping.Sum(x => x.Price * x.Quantity);
            var taxTotal = itemization.Tax.Sum(x => x.Price * x.Quantity);
            var adjTotal = itemization.Adjustments.Sum(x => x.Price * x.Quantity);
            var discountsTotal = itemization.Discounts.Sum(x => x.Price * x.Quantity);
            var customTotal = itemization.Custom.Sum(x => x.Price * x.Quantity);

            return productTotal + shippingTotal + taxTotal + customTotal - (discountsTotal + adjTotal);
        }
    }
}