using Merchello.Core.Checkout;
using Merchello.Core.Models;

namespace Merchello.Core.Chains.InvoiceCreation
{
    public abstract class CheckoutPreparationAttemptChainTaskBase : AttemptChainTaskBase<IInvoice>
    {
        private readonly CheckoutPreparationBase _checkoutPreparation;

        protected CheckoutPreparationAttemptChainTaskBase(CheckoutPreparationBase checkoutPreparation)
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