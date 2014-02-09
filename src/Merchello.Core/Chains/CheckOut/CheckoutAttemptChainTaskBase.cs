using Merchello.Core.Checkout;
using Merchello.Core.Models;
using Umbraco.Core;

namespace Merchello.Core.Chains.CheckOut
{
    public abstract class CheckoutAttemptChainTaskBase : AttemptChainTaskBase<IInvoice>
    {
        private readonly CheckoutPreparationBase _checkoutPreparation;

        protected CheckoutAttemptChainTaskBase(CheckoutPreparationBase checkoutPreparation)
        {
            Mandate.ParameterNotNull(checkoutPreparation, "checkout");

            _checkoutPreparation = checkoutPreparation;
        }

        /// <summary>
        /// Gets the <see cref="CheckoutPreparationBase"/> object
        /// </summary>
        protected CheckoutPreparationBase CheckoutPreparation
        {
            get { return _checkoutPreparation; }
        }
    }
}