using Merchello.Core.Checkout;
using Merchello.Core.Models;
using Umbraco.Core;

namespace Merchello.Core.Chains.CheckOut
{
    internal class AddShipmentLineItemsToInvoiceTask : CheckoutAttemptChainTaskBase
    {
        public AddShipmentLineItemsToInvoiceTask(CheckoutPreparationBase checkoutPreparation) 
            : base(checkoutPreparation)
        { }


        /// <summary>
        /// Adds billing information to the invoice
        /// </summary>
        /// <param name="value">The <see cref="IInvoice"/></param>
        /// <returns></returns>
        public override Attempt<IInvoice> PerformTask(IInvoice value)
        {

            return Attempt<IInvoice>.Succeed(value);
        }
    }
}