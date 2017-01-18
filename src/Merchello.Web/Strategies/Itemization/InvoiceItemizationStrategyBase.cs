namespace Merchello.Web.Strategies.Itemization
{
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Logging;
    using Merchello.Core.Models;
    using Merchello.Core.Strategies;
    using Merchello.Web.Models.ContentEditing;

    /// <summary>
    /// The invoice itemization strategy base.
    /// </summary>
    public abstract class InvoiceItemizationStrategyBase : IStrategy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceItemizationStrategyBase"/> class.
        /// </summary>
        /// <param name="display">
        /// The <see cref="InvoiceDisplay"/>.
        /// </param>
        protected InvoiceItemizationStrategyBase(InvoiceDisplay display)
        {
            Ensure.ParameterNotNull(display, "invoice");
            Invoice = display;
        }

        /// <summary>
        /// Gets the invoice.
        /// </summary>
        protected InvoiceDisplay Invoice { get; }

        /// <summary>
        /// Itemizes the invoice.
        /// </summary>
        /// <returns>
        /// The <see cref="InvoiceItemizationDisplay"/>.
        /// </returns>
        public InvoiceItemizationDisplay Itemize()
        {
            var itemization = ItemizeInvoice();
            itemization.Reconciles = true;

            if (!this.Reconcile(itemization))
            {
                itemization.Reconciles = false;
                MultiLogHelper.Warn<InvoiceItemizationStrategyBase>("Reconciliation of invoice total failed in the itemization strategy");
            }

            itemization.Total = Invoice.Total;

            return itemization;
        }

        /// <summary>
        /// Itemizes the invoice.
        /// </summary>
        /// <returns>
        /// The <see cref="InvoiceItemizationDisplay"/>.
        /// </returns>
        protected abstract InvoiceItemizationDisplay ItemizeInvoice();

        /// <summary>
        /// Gets the line item collection by <see cref="LineItemType"/>.
        /// </summary>
        /// <param name="lineItemType">
        /// The line item type.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{InvoiceLineItem}"/>.
        /// </returns>
        protected virtual IEnumerable<InvoiceLineItemDisplay> GetLineItemCollection(LineItemType lineItemType)
        {
            return Invoice.Items.Where(x => x.LineItemType == lineItemType);
        }

        /// <summary>
        /// Reconciles the itemization.
        /// </summary>
        /// <param name="itemization">
        /// The itemization.
        /// </param>
        /// <returns>
        /// A value that indicates that the itemization could be reconciled.
        /// </returns>
        protected virtual bool Reconcile(InvoiceItemizationDisplay itemization)
        {
            return itemization.CalculateTotal() == Invoice.Total;
        }
    }
}