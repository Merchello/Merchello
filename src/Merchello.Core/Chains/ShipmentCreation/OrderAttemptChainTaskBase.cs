using Merchello.Core.Models;

namespace Merchello.Core.Chains.ShipmentCreation
{
    public abstract class OrderAttemptChainTaskBase : AttemptChainTaskBase<IShipment>
    {
        private readonly IMerchelloContext _merchelloContext;
        private readonly IOrder _order;

        protected OrderAttemptChainTaskBase(IMerchelloContext merchelloContext, IOrder order)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            Mandate.ParameterNotNull(order, "order");

            _merchelloContext = merchelloContext;
            _order = order;
        }

        /// <summary>
        /// Gets the <see cref="IMerchelloContext"/>
        /// </summary>
        public IMerchelloContext MerchelloContext
        {
            get { return _merchelloContext; }
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