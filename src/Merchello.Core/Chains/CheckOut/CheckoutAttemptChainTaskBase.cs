using Merchello.Core.Checkout;
using Merchello.Core.Models;
using Umbraco.Core;

namespace Merchello.Core.Chains.CheckOut
{
    public abstract class CheckoutAttemptChainTaskBase : IAttemptChainTask<IInvoice>
    {
        private readonly CheckoutBase _checkout;

        protected CheckoutAttemptChainTaskBase(CheckoutBase checkout)
        {
            Mandate.ParameterNotNull(checkout, "checkout");

            _checkout = checkout;
        }

        /// <summary>
        /// Attempts to perform a task on an invoice.
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/> to perform the task on</param>
        /// <returns>Returns an <see cref="Attempt"/> of a task completion</returns>
        public abstract Attempt<IInvoice> PerformTask(IInvoice invoice);

        /// <summary>
        /// Gets the <see cref="CheckoutBase"/> object
        /// </summary>
        protected CheckoutBase Checkout
        {
            get { return _checkout; }
        }
    }
}