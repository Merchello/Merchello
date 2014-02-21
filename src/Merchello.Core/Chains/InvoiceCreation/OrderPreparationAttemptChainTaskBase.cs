using Merchello.Core.Models;
using Merchello.Core.Orders;

namespace Merchello.Core.Chains.InvoiceCreation
{
    public abstract class OrderPreparationAttemptChainTaskBase : AttemptChainTaskBase<IInvoice>
    {
        private readonly OrderPreparationBase _orderPreparation;

        protected OrderPreparationAttemptChainTaskBase(OrderPreparationBase orderPreparation)
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