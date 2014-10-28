namespace Merchello.Core.Builders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Models;
    using Umbraco.Core;

    /// <summary>
    /// Builds a shipment
    /// </summary>
    internal sealed class ShipmentBuilderChain : BuildChainBase<IShipment>
    {
        /// <summary>
        /// The _order.
        /// </summary>
        private readonly IOrder _order;

        /// <summary>
        /// A collection of order line item keys to be included in the shipment
        /// </summary>
        private readonly IEnumerable<Guid> _keysToShip;
 
        /// <summary>
        /// The merchello context.
        /// </summary>
        private readonly IMerchelloContext _merchelloContext;

        /// <summary>
        /// Constructor parameters for the base class activator
        /// </summary>
        private IEnumerable<object> _constructorParameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShipmentBuilderChain"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="order">
        /// The order.
        /// </param>
        /// <param name="keysToShip">
        /// A collection of line item keys which identifies which line items in the order are to be included in the shipment being packaged
        /// </param>
        public ShipmentBuilderChain(IMerchelloContext merchelloContext, IOrder order, IEnumerable<Guid> keysToShip)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            Mandate.ParameterNotNull(order, "order");
            Mandate.ParameterNotNull(keysToShip, "keysToShip");

            _merchelloContext = merchelloContext;
            _order = order;
            _keysToShip = keysToShip;

            ResolveChain(Core.Constants.TaskChainAlias.OrderPreparationShipmentCreate);
        }

        /// <summary>
        /// Gets the task count. Used for testing
        /// </summary>
        internal int TaskCount
        {
            get { return TaskHandlers.Count(); }
        }

        /// <summary>
        /// Gets the constructor argument values.
        /// </summary>
        protected override IEnumerable<object> ConstructorArgumentValues
        {
            get
            {
                return _constructorParameters ?? (_constructorParameters = new List<object>(new object[] { _merchelloContext, _order, _keysToShip }));
            }
        }

        /// <summary>
        /// Builds the order
        /// </summary>
        /// <returns>The Attempt{IShipment}</returns>
        public override Attempt<IShipment> Build()
        {            
            // invoice 
            var invoice = _merchelloContext.Services.InvoiceService.GetByKey(_order.InvoiceKey);
            if (invoice == null) return Attempt<IShipment>.Fail(new NullReferenceException("An invoice could not be found for the order passed"));
            
            // shipment line item
            var shipmentLineItem = invoice.Items.FirstOrDefault(x => x.LineItemType == LineItemType.Shipping);
            if (shipmentLineItem == null) return Attempt<IShipment>.Fail(new NullReferenceException("An shipment could not be found in the invoice assoiciated with the order passed"));

            // the shipment
            var quoted = shipmentLineItem.ExtendedData.GetShipment<InvoiceLineItem>();
            if (quoted == null) return Attempt<IShipment>.Fail(new NullReferenceException("An shipment could not be found in the invoice assoiciated with the order passed"));

            var quotedStatus = _merchelloContext.Services.ShipmentService.GetShipmentStatusByKey(Core.Constants.DefaultKeys.ShipmentStatus.Quoted);

            // execute the change
            var attempt = TaskHandlers.Any()
                ? TaskHandlers.First().Execute(
                        new Shipment(quotedStatus, quoted.GetOriginAddress(), quoted.GetDestinationAddress())
                            {
                                ShipMethodKey = quoted.ShipMethodKey,
                                VersionKey = quoted.VersionKey
                            })
                : Attempt<IShipment>.Fail(new InvalidOperationException("The configuration Chain Task List could not be instantiated."));

            return attempt;
        }       
    }
}