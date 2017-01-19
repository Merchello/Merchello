namespace Merchello.Core.Strategies.Itemization
{
    using Merchello.Core.Models;

    /// <summary>
    /// Represents the invoice itemization strategy for splitting out taxes (VAT) that were included in product pricing.
    /// </summary>
    public class ProductBasedTaxationInvoiceItemazationStrategy : InvoiceItemizationStrategyBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductBasedTaxationInvoiceItemazationStrategy"/> class.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        public ProductBasedTaxationInvoiceItemazationStrategy(Invoice invoice)
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