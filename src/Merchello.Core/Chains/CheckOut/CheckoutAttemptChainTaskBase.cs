using Merchello.Core.Checkout;
using Merchello.Core.Models;
using Umbraco.Core;

namespace Merchello.Core.Chains.CheckOut
{
    public abstract class CheckoutAttemptChainTaskBase : AttemptChainTaskBase<IInvoice>
    {
        private readonly CheckoutBase _checkout;

        protected CheckoutAttemptChainTaskBase(CheckoutBase checkout)
        {
            Mandate.ParameterNotNull(checkout, "checkout");

            _checkout = checkout;
        }

        /// <summary>
        /// Gets the <see cref="CheckoutBase"/> object
        /// </summary>
        protected CheckoutBase Checkout
        {
            get { return _checkout; }
        }
    }
}