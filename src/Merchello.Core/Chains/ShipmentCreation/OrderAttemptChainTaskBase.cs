namespace Merchello.Core.Chains.ShipmentCreation
{
    using System;
    using System.Collections.Generic;
    using Models;

    /// <summary>
    /// The order attempt chain task base.
    /// </summary>
    public abstract class OrderAttemptChainTaskBase : AttemptChainTaskBase<IShipment>
    {
        /// <summary>
        /// The _merchello context.
        /// </summary>
        private readonly IMerchelloContext _merchelloContext;

        /// <summary>
        /// The _order.
        /// </summary>
        private readonly IOrder _order;

        /// <summary>
        /// The collection of keys associated with the order line items that are to be added to the shipment
        /// </summary>
        private readonly IEnumerable<Guid> _keysToShip;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderAttemptChainTaskBase"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="order">
        /// The order.
        /// </param>
        /// <param name="keysToShip">
        /// The keys to ship.
        /// </param>
        protected OrderAttemptChainTaskBase(IMerchelloContext merchelloContext, IOrder order, IEnumerable<Guid> keysToShip)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            Mandate.ParameterNotNull(order, "order");
            Mandate.ParameterNotNull(keysToShip, "keysToShip");

            _merchelloContext = merchelloContext;
            _order = order;
            _keysToShip = keysToShip;
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

        /// <summary>
        /// Gets the keys to ship.
        /// </summary>
        public IEnumerable<Guid> KeysToShip
        {
            get { return _keysToShip; }
        }

    }
}