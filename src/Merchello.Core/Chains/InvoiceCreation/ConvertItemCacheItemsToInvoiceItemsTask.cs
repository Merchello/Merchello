using System;
using Merchello.Core.Checkout;
using Merchello.Core.Models;
using Umbraco.Core;

namespace Merchello.Core.Chains.InvoiceCreation
{
    /// <summary>
    /// Converts ItemCacheLineItem(s) to InvoiceLineItems
    /// </summary>
    internal class ConvertItemCacheItemsToInvoiceItemsTask : CheckoutPreparationAttemptChainTaskBase
    {
        public ConvertItemCacheItemsToInvoiceItemsTask(OrderPreparationBase orderPreparation) 
            : base(orderPreparation)
        {}

        /// <summary>
        /// Task converts ItemCacheLineItems to InvoiceLineItems and adds them to the invoice
        /// </summary>
        /// <param name="value">The <see cref="IInvoice"/> to which to add the line items</param>
        /// <returns>The <see cref="Attempt"/></returns>
        public override Attempt<IInvoice> PerformTask(IInvoice value)
        {
            foreach (var lineItem in OrderPreparation.ItemCache.Items)
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