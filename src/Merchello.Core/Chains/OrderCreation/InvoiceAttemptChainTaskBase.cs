using Merchello.Core.Models;

namespace Merchello.Core.Chains.OrderCreation
{
    public abstract class InvoiceAttemptChainTaskBase : AttemptChainTaskBase<IOrder>
    {
        private readonly IInvoice _invoice;

        protected InvoiceAttemptChainTaskBase(IInvoice invoice)
        {
            Mandate.ParameterNotNull(invoice, "invoice");

            _invoice = invoice;
        }

        /// <summary>
        /// Gets the <see cref="IInvoice"/> object
        /// </summary>
        protected IInvoice Invoice
        {
            get { return _invoice; }
        }
    }
}