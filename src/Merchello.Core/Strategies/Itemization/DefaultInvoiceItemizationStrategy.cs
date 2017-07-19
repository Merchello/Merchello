namespace Merchello.Core.Strategies.Itemization
{
    using Merchello.Core.Models;

    /// <summary>
    /// Represents the default invoice itemization strategy.
    /// </summary>
    public class DefaultInvoiceItemizationStrategy : InvoiceItemizationStrategyBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultInvoiceItemizationStrategy"/> class.
        /// </summary>
        /// <param name="invoice">
        /// The display.
        /// </param>
        public DefaultInvoiceItemizationStrategy(Invoice invoice)
            : base(invoice)
        {
        }


        /// <summary>
        /// Itemizes the invoice.
        /// </summary>
        /// <returns>
        /// The <see cref="InvoiceItemItemization"/>.
        /// </returns>
        protected override InvoiceItemItemization ItemizeInvoice()
        {
            return new InvoiceItemItemization
            {
                Products = GetLineItemCollection(LineItemType.Product),
                Tax = GetLineItemCollection(LineItemType.Tax),
                Adjustments = GetLineItemCollection(LineItemType.Adjustment),
                Shipping = GetLineItemCollection(LineItemType.Shipping),
                Custom = GetLineItemCollection(LineItemType.Custom),
                Discounts = GetLineItemCollection(LineItemType.Discount)
            };
        }
    }
}