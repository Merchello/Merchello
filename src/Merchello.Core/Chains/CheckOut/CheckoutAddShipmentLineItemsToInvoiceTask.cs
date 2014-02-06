using Merchello.Core.Checkout;
using Merchello.Core.Models;
using Umbraco.Core;

namespace Merchello.Core.Chains.CheckOut
{
    internal class CheckoutAddShipmentLineItemsToInvoiceTask : CheckoutAttemptChainTaskBase
    {
        public CheckoutAddShipmentLineItemsToInvoiceTask(CheckoutBase checkout) 
            : base(checkout)
        { }

        public override Attempt<IInvoice> PerformTask(IInvoice invoice)
        {
            throw new System.NotImplementedException();
        }
    }
}