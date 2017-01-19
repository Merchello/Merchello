namespace Merchello.Core.Strategies.Itemization
{
    using System.Linq;

    using Merchello.Core.Models;

    /// <summary>
    /// Extension methods for <see cref="InvoiceItemItemization"/>.
    /// </summary>
    public static class InvoiceItemizationExtensions
    {
        /// <summary>
        /// Calculates the product total.
        /// </summary>
        /// <param name="itemization">
        /// The itemization.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        public static decimal CalculateProductTotal(this InvoiceItemItemization itemization)
        {
            return InvoiceExtensions.Ensure2Places(itemization.Products.Sum(x => x.Price * x.Quantity));
        }

        /// <summary>
        /// Calculates the  shipping total.
        /// </summary>
        /// <param name="itemization">
        /// The itemization.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        public static decimal CalculateShippingTotal(this InvoiceItemItemization itemization)
        {
            return InvoiceExtensions.Ensure2Places(itemization.Shipping.Sum(x => x.Price * x.Quantity));
        }

        /// <summary>
        /// Calculates the tax total.
        /// </summary>
        /// <param name="itemization">
        /// The itemization.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        public static decimal CalculateTaxTotal(this InvoiceItemItemization itemization)
        {
            return InvoiceExtensions.Ensure2Places(itemization.Tax.Sum(x => x.Price * x.Quantity));
        }

        /// <summary>
        /// Calculates the adjustment total.
        /// </summary>
        /// <param name="itemization">
        /// The itemization.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        public static decimal CalculateAdjustmentTotal(this InvoiceItemItemization itemization)
        {
            return InvoiceExtensions.Ensure2Places(itemization.Adjustments.Sum(x => x.Price * x.Quantity));
        }

        /// <summary>
        /// Calculates the discount total.
        /// </summary>
        /// <param name="itemization">
        /// The itemization.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        public static decimal CalculateDiscountTotal(this InvoiceItemItemization itemization)
        {
            return InvoiceExtensions.Ensure2Places(itemization.Discounts.Sum(x => x.Price * x.Quantity));
        }

        /// <summary>
        /// Calculates the custom total.
        /// </summary>
        /// <param name="itemization">
        /// The itemization.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        public static decimal CalculateCustomTotal(this InvoiceItemItemization itemization)
        {
            return InvoiceExtensions.Ensure2Places(itemization.Custom.Sum(x => x.Price * x.Quantity));
        }

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
            var productTotal = itemization.CalculateProductTotal();
            var shippingTotal = itemization.CalculateShippingTotal();
            var taxTotal = itemization.CalculateTaxTotal();
            var adjTotal = itemization.CalculateAdjustmentTotal();
            var discountsTotal = itemization.CalculateDiscountTotal();
            var customTotal = itemization.CalculateCustomTotal();

            return productTotal + shippingTotal + taxTotal + customTotal + adjTotal - discountsTotal;
        }
    }
}