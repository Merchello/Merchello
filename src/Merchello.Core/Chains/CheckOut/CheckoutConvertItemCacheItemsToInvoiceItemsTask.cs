using System;
using Merchello.Core.Checkout;
using Merchello.Core.Models;
using Umbraco.Core;

namespace Merchello.Core.Chains.CheckOut
{
    /// <summary>
    /// Converts ItemCacheLineItem(s) to InvoiceLineItems
    /// </summary>
    internal class CheckoutConvertItemCacheItemsToInvoiceItemsTask : CheckoutAttemptChainTaskBase
    {
        public CheckoutConvertItemCacheItemsToInvoiceItemsTask(CheckoutBase checkout) 
            : base(checkout)
        {}

        /// <summary>
        /// Task converts ItemCacheLineItems to InvoiceLineItems and adds them to the invoice
        /// </summary>
        /// <param name="value">The <see cref="IInvoice"/> to which to add the line items</param>
        /// <returns>The <see cref="Attempt"/></returns>
        public override Attempt<IInvoice> PerformTask(IInvoice value)
        {
            foreach (var lineItem in Checkout.ItemCache.Items)
            {
                try
                {
                    value.Items.Add(lineItem.ConvertToNewLineItemOf<InvoiceLineItem>());
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