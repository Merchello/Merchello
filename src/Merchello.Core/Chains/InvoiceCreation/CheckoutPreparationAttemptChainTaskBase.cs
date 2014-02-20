using Merchello.Core.Checkout;
using Merchello.Core.Models;

namespace Merchello.Core.Chains.InvoiceCreation
{
    public abstract class CheckoutPreparationAttemptChainTaskBase : AttemptChainTaskBase<IInvoice>
    {
        private readonly OrderPreparationBase _orderPreparation;

        protected CheckoutPreparationAttemptChainTaskBase(OrderPreparationBase orderPreparation)
        {
            Mandate.ParameterNotNull(orderPreparation, "checkout");

            _orderPreparation = orderPreparation;
        }

        /// <summary>
        /// Gets the <see cref="OrderPreparationBase"/> object
        /// </summary>
        protected OrderPreparationBase OrderPreparation
        {
            get { return _orderPreparation; }
        }
    }
}