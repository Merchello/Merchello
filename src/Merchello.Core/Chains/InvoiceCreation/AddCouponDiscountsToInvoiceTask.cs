namespace Merchello.Core.Chains.InvoiceCreation
{
    using Merchello.Core.Models;
    using Merchello.Core.Sales;

    using Umbraco.Core;

    public class AddCouponDiscountsToInvoiceTask : InvoiceCreationAttemptChainTaskBase
    {
        public AddCouponDiscountsToInvoiceTask(SalePreparationBase salePreparation)
            : base(salePreparation)
        {
        }

        public override Attempt<IInvoice> PerformTask(IInvoice value)
        {
            throw new System.NotImplementedException();
        }
    }
}