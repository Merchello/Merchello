using Merchello.Core.Models;

namespace Merchello.Core.Chains.InvoiceTaxRateQuote
{
    public abstract class InvoiceTaxRateQuoteBase : AttemptChainTaskBase<IInvoice>
    {
        private readonly IInvoice _invoice;

        protected InvoiceTaxRateQuoteBase(IInvoice invoice)
        {
            Mandate.ParameterNotNull(invoice, "invoice");

            _invoice = invoice;
        }

        /// <summary>
        /// Gets the <see cref="IInvoice"/>
        /// </summary>
        protected IInvoice Invoice
        {
            get { return _invoice; }
        }
    }
}