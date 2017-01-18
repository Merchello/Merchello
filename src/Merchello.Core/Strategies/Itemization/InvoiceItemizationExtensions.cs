namespace Merchello.Core.Strategies.Itemization
{
    using System.Linq;

    using Merchello.Core.Models;

    /// <summary>
    /// Extension methods for <see cref="InvoiceItemItemization"/>.
    /// </summary>
    internal static class InvoiceItemizationExtensions
    {
        /// <summary>
        /// Calculates the invoice item total.
        /// </summary>
        /// <param name="itemization">
        /// The itemization.
        /// </param>
        /// <returns>
        /// The total.
        /// </returns>
        public static decimal CalculateTotal(this InvoiceItemItemization itemization)
        {
            var productTotal = itemization.Products.Sum(x => x.Price * x.Quantity);
            var shippingTotal = itemization.Shipping.Sum(x => x.Price * x.Quantity);
            var taxTotal = itemization.Tax.Sum(x => x.Price * x.Quantity);
            var adjTotal = itemization.Adjustments.Sum(x => x.Price * x.Quantity);
            var discountsTotal = itemization.Discounts.Sum(x => x.Price * x.Quantity);
            var customTotal = itemization.Custom.Sum(x => x.Price * x.Quantity);

            return InvoiceExtensions.Ensure2Places(productTotal + shippingTotal + taxTotal + customTotal + adjTotal - discountsTotal);
        }
    }
}