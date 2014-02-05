using Merchello.Core.Checkout;
using Merchello.Core.Models;
using Umbraco.Core;

namespace Merchello.Core.Chains.CheckOut
{
    internal class CheckoutApplyTaxesToInvoiceTax : CheckoutAttemptChainTaskBase
    {
        internal CheckoutApplyTaxesToInvoiceTax(CheckoutBase checkout) 
            : base(checkout)
        { }

        public override Attempt<IInvoice> PerformTask(IInvoice invoice)
        {
            throw new System.NotImplementedException();
        }
    }
}