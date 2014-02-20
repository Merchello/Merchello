using Merchello.Core.Checkout;
using Merchello.Core.Models;
using Umbraco.Core;

namespace Merchello.Core.Chains.InvoiceCreation
{
    internal class ApplyTaxesToInvoiceTax : CheckoutPreparationAttemptChainTaskBase
    {
        public ApplyTaxesToInvoiceTax(CheckoutPreparationBase checkoutPreparation) 
            : base(checkoutPreparation)
        { }

        public override Attempt<IInvoice> PerformTask(IInvoice value)
        {
            // if taxes are not to be applied, skip this step
            if(!CheckoutPreparation.ApplyTaxesToInvoice) return Attempt<IInvoice>.Succeed(value);

            return Attempt<IInvoice>.Succeed(value);
        }
    }
}