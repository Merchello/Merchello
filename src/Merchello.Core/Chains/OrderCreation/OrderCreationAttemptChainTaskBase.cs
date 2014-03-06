using Merchello.Core.Models;

namespace Merchello.Core.Chains.OrderCreation
{
    public abstract class OrderCreationAttemptChainTaskBase : AttemptChainTaskBase<IOrder>
    {
        private readonly IInvoice _invoice;

        protected OrderCreationAttemptChainTaskBase(IInvoice invoice)
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