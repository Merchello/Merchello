namespace Merchello.Core.Chains.OrderCreation
{
    using System;
    using System.Linq;
    using Models;
    using Umbraco.Core;

    /// <summary>
    /// The convert invoice items to order items task.
    /// </summary>
    internal class ConvertInvoiceItemsToOrderItemsTask : OrderCreationAttemptChainTaskBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConvertInvoiceItemsToOrderItemsTask"/> class.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        public ConvertInvoiceItemsToOrderItemsTask(IInvoice invoice)
            : base(invoice)
        {            
        }

        /// <summary>
        /// Task converts InvoiceLineItems to OrderLineItems and adds them to the order
        /// </summary>
        /// <param name="value">The <see cref="IOrder"/> to which to add the line items</param>
        /// <returns>The <see cref="Attempt"/></returns>
        public override Attempt<IOrder> PerformTask(IOrder value)
        {
            var items = Invoice.Items.Where(
                        x =>
                            x.LineItemType != LineItemType.Shipping && x.LineItemType != LineItemType.Tax &&
                            x.LineItemType != LineItemType.Discount);
            
            foreach (var item in items)                    
            {
                try
                {
                    value.Items.Add(item.AsLineItemOf<OrderLineItem>());
                }
                catch (Exception ex)
                {
                    return Attempt<IOrder>.Fail(ex);
                }                
            }

            return Attempt<IOrder>.Succeed(value);
        }
    }
}