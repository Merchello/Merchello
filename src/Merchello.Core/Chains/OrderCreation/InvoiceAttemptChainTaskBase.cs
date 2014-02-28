using Merchello.Core.Models;

namespace Merchello.Core.Chains.OrderCreation
{
    public abstract class InvoiceAttemptChainTaskBase : AttemptChainTaskBase<IOrder>
    {
        private readonly IMerchelloContext _merchelloContext;
        private readonly IInvoice _invoice;

        protected InvoiceAttemptChainTaskBase(IMerchelloContext merchelloContext, IInvoice invoice)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            Mandate.ParameterNotNull(invoice, "invoice");

            _invoice = invoice;
            _merchelloContext = merchelloContext;
        }

        /// <summary>
        /// Gets the <see cref="IInvoice"/> object
        /// </summary>
        protected IInvoice Invoice
        {
            get { return _invoice; }
        }

        /// <summary>
        /// Gets the <see cref="IMerchelloContext"/>
        /// </summary>
        protected IMerchelloContext MerchelloContext
        {
            get { return _merchelloContext; }
        }
    }
}