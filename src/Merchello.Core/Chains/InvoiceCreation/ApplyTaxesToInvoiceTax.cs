using Merchello.Core.Checkout;
using Merchello.Core.Models;
using Umbraco.Core;

namespace Merchello.Core.Chains.InvoiceCreation
{
    internal class ApplyTaxesToInvoiceTax : CheckoutPreparationAttemptChainTaskBase
    {
        public ApplyTaxesToInvoiceTax(OrderPreparationBase orderPreparation) 
            : base(orderPreparation)
        { }

        public override Attempt<IInvoice> PerformTask(IInvoice value)
        {
            // if taxes are not to be applied, skip this step
            if(!OrderPreparation.ApplyTaxesToInvoice) return Attempt<IInvoice>.Succeed(value);

            return Attempt<IInvoice>.Succeed(value);
        }
    }
}