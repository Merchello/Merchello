namespace Merchello.Core.Chains.InvoiceCreation.SalesPreparation
{
    using System;

    using Merchello.Core.Models;
    using Merchello.Core.Sales;

    using Umbraco.Core;

    /// <summary>
    /// Converts ItemCacheLineItem(s) to InvoiceLineItems
    /// </summary>
    [Obsolete("Superseded by CheckoutManger.ConvertItemCacheItemsToInvoiceItemsTask")]
    internal class ConvertItemCacheItemsToInvoiceItemsTask : InvoiceCreationAttemptChainTaskBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConvertItemCacheItemsToInvoiceItemsTask"/> class.
        /// </summary>
        /// <param name="salePreparation">
        /// The sale preparation.
        /// </param>
        public ConvertItemCacheItemsToInvoiceItemsTask(SalePreparationBase salePreparation)
            : base(salePreparation)
        {
        }

        /// <summary>
        /// Task converts ItemCacheLineItems to InvoiceLineItems and adds them to the invoice
        /// </summary>
        /// <param name="value">The <see cref="IInvoice"/> to which to add the line items</param>
        /// <returns>The <see cref="Attempt"/></returns>
        public override Attempt<IInvoice> PerformTask(IInvoice value)
        {
            foreach (var lineItem in this.SalePreparation.ItemCache.Items)
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