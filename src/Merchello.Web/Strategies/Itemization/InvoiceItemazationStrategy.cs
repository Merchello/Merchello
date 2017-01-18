namespace Merchello.Web.Strategies.Itemization
{
    using Merchello.Core.Models;
    using Merchello.Web.Models.ContentEditing;

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
        public DefaultInvoiceItemazationStrategy(InvoiceDisplay invoice)
            : base(invoice)
        {
        }

        /// <summary>
        /// Itemizes the invoice.
        /// </summary>
        /// <returns>
        /// The <see cref="InvoiceItemizationDisplay"/>.
        /// </returns>
        protected override InvoiceItemizationDisplay ItemizeInvoice()
        {
            throw new System.NotImplementedException();
        }
    }
}