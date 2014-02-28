using System.Linq;
using Merchello.Core.Models;
using Umbraco.Core;

namespace Merchello.Core.Chains.OrderCreation
{
    internal class ConvertInvoiceItemsToOrderItems : InvoiceAttemptChainTaskBase
    {
        public ConvertInvoiceItemsToOrderItems(IInvoice invoice) 
            : base(invoice)
        {}

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
                value.Items.Add(item.AsLineItemOf<IOrderLineItem>());
            }

            return Attempt<IOrder>.Succeed(value);
        }
    }
}