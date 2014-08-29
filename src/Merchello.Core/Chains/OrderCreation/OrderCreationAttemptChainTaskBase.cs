namespace Merchello.Core.Chains.OrderCreation
{
    using Models;

    /// <summary>
    /// The order creation attempt chain task base.
    /// </summary>
    public abstract class OrderCreationAttemptChainTaskBase : AttemptChainTaskBase<IOrder>
    {
        /// <summary>
        /// The invoice.
        /// </summary>
        private readonly IInvoice _invoice;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderCreationAttemptChainTaskBase"/> class.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
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