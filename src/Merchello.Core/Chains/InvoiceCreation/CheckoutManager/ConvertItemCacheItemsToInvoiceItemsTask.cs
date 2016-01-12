namespace Merchello.Core.Chains.InvoiceCreation.CheckoutManager
{
    using System;

    using Merchello.Core.Checkout;
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// Converts ItemCacheLineItem(s) to InvoiceLineItems
    /// </summary>
    internal class ConvertItemCacheItemsToInvoiceItemsTask : CheckoutManagerInvoiceCreationAttemptChainTaskBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConvertItemCacheItemsToInvoiceItemsTask"/> class.
        /// </summary>
        /// <param name="checkoutManager">
        /// The <see cref="ICheckoutManagerBase"/>.
        /// </param>
        public ConvertItemCacheItemsToInvoiceItemsTask(ICheckoutManagerBase checkoutManager)
            : base(checkoutManager)
        {
        }

        /// <summary>
        /// Task converts ItemCacheLineItems to InvoiceLineItems and adds them to the invoice
        /// </summary>
        /// <param name="value">The <see cref="IInvoice"/> to which to add the line items</param>
        /// <returns>The <see cref="Attempt"/></returns>
        public override Attempt<IInvoice> PerformTask(IInvoice value)
        {
            foreach (var lineItem in this.CheckoutManager.Context.ItemCache.Items)
            {
                try
                {
                    value.Items.Add(lineItem.AsLineItemOf<InvoiceLineItem>());
                }
                catch (Exception ex)
                {
                    return Attempt<IInvoice>.Fail(ex);
                }
            }

            return Attempt<IInvoice>.Succeed(value);
        }
    }
}