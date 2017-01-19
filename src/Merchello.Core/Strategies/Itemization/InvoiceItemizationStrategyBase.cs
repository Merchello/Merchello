namespace Merchello.Core.Strategies.Itemization
{
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Logging;
    using Merchello.Core.Models;
    using Merchello.Core.Strategies;

    /// <summary>
    /// The invoice itemization strategy base.
    /// </summary>
    public abstract class InvoiceItemizationStrategyBase : IStrategy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceItemizationStrategyBase"/> class.
        /// </summary>
        /// <param name="display">
        /// The <see cref="Invoice"/>.
        /// </param>
        protected InvoiceItemizationStrategyBase(Invoice display)
        {
            Ensure.ParameterNotNull(display, "invoice");
            this.Invoice = display;
        }

        /// <summary>
        /// Gets the invoice.
        /// </summary>
        protected Invoice Invoice { get; }

        /// <summary>
        /// Itemizes the invoice.
        /// </summary>
        /// <returns>
        /// The <see cref="InvoiceItemItemization"/>.
        /// </returns>
        public InvoiceItemItemization Itemize()
        {
            var itemization = this.ItemizeInvoice();
            itemization.Reconciles = true;

            if (!this.Reconcile(itemization))
            {
                itemization.Reconciles = false;
                MultiLogHelper.Warn<InvoiceItemizationStrategyBase>("Reconciliation of invoice total failed in the itemization strategy");
            }

            itemization.ItemizationTotal = itemization.CalculateTotal();
            itemization.InvoiceTotal = Invoice.Total;
            itemization.ProductTotal = itemization.CalculateProductTotal();
            itemization.ShippingTotal = itemization.CalculateShippingTotal();
            itemization.TaxTotal = itemization.CalculateTaxTotal();
            itemization.AdjustmentTotal = itemization.CalculateAdjustmentTotal();
            itemization.DiscountTotal = itemization.CalculateDiscountTotal();
            itemization.CustomTotal = itemization.CalculateCustomTotal();

            return itemization;
        }

        /// <summary>
        /// Itemizes the invoice.
        /// </summary>
        /// <returns>
        /// The <see cref="InvoiceItemItemization"/>.
        /// </returns>
        protected abstract InvoiceItemItemization ItemizeInvoice();

        /// <summary>
        /// Gets the line item collection by <see cref="LineItemType"/>.
        /// </summary>
        /// <param name="lineItemType">
        /// The line item type.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{InvoiceLineItem}"/>.
        /// </returns>
        protected virtual IEnumerable<ILineItem> GetLineItemCollection(LineItemType lineItemType)
        {
            return this.Invoice.Items.Where(x => x.LineItemType == lineItemType).Select(x => x.AsLineItemWithKeyOf<InvoiceLineItem>());
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
        protected virtual bool Reconcile(InvoiceItemItemization itemization)
        {
            return itemization.CalculateTotal() == this.Invoice.Total;
        }
    }
}