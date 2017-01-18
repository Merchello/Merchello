namespace Merchello.Core.Strategies.Itemization
{
    using Merchello.Core.Models;

    /// <summary>
    /// Represents the default invoice itemization strategy.
    /// </summary>
    public class DefaultInvoiceItemazationStrategy : InvoiceItemizationStrategyBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultInvoiceItemazationStrategy"/> class.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        public DefaultInvoiceItemazationStrategy(Invoice invoice)
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
            var visitor = new ProductBasedTaxationVisitor();
            Invoice.Accept(visitor);

            return new InvoiceItemItemization
                {
                    Products = visitor.ProductLineItems,
                    Tax = visitor.TaxLineItems,
                    Adjustments = GetLineItemCollection(LineItemType.Adjustment),
                    Shipping = GetLineItemCollection(LineItemType.Shipping),
                    Custom = GetLineItemCollection(LineItemType.Custom),
                    Discounts = GetLineItemCollection(LineItemType.Discount)
                };
        }
    }
}