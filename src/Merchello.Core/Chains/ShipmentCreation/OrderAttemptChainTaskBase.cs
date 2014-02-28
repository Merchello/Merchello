using Merchello.Core.Models;
using Umbraco.Core;

namespace Merchello.Core.Chains.ShipmentCreation
{
    public abstract class OrderAttemptChainTaskBase : AttemptChainTaskBase<IShipment>
    {
        private readonly IOrder _order;

        protected OrderAttemptChainTaskBase(IOrder order)
        {
            Mandate.ParameterNotNull(order, "order");

            _order = order;
        }

        /// <summary>
        /// Gets the <see cref="IOrder"/>
        /// </summary>
        public IOrder Order
        {
            get { return _order; }
        }

    }
}