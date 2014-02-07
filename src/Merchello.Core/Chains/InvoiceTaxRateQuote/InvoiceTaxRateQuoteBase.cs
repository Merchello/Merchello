using Merchello.Core.Models;

namespace Merchello.Core.Chains.InvoiceTaxRateQuote
{
    public abstract class InvoiceTaxRateQuoteBase : AttemptChainTaskBase<IInvoice>
    {
        private readonly IInvoice _invoice;

    }
}