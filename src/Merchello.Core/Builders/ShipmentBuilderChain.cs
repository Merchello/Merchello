using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Models;
using Umbraco.Core;

namespace Merchello.Core.Builders
{
    /// <summary>
    /// Builds a shipment
    /// </summary>
    internal sealed class ShipmentBuilderChain : BuildChainBase<IShipment>
    {
        private readonly IOrder _order;
        private readonly IMerchelloContext _merchelloContext;

        public ShipmentBuilderChain(IMerchelloContext merchelloContext, IOrder order)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            Mandate.ParameterNotNull(order, "order");

            _merchelloContext = merchelloContext;
            _order = order;

            ResolveChain(Constants.TaskChainAlias.OrderPreparationShipmentCreate);
        }

        /// <summary>
        /// Builds the order
        /// </summary>
        /// <returns>Attempt{IShipment}</returns>
        public override Attempt<IShipment> Build()
        {
            // invoice 
            var invoice = _merchelloContext.Services.InvoiceService.GetByKey(_order.InvoiceKey);
            if(invoice == null) return Attempt<IShipment>.Fail(new NullReferenceException("An invoice could not be found for the order passed"));
            
            // shipment line item
            var shipmentLineItem = invoice.Items.FirstOrDefault(x => x.LineItemType == LineItemType.Shipping);
            if(shipmentLineItem == null) return Attempt<IShipment>.Fail(new NullReferenceException("An shipment could not be found in the invoice assoiciated with the order passed"));

            // the shipment
            var quoted = shipmentLineItem.ExtendedData.GetShipment<InvoiceLineItem>();
            if (quoted == null) return Attempt<IShipment>.Fail(new NullReferenceException("An shipment could not be found in the invoice assoiciated with the order passed"));

            // execute the change
            var attempt = (TaskHandlers.Any())
                ? TaskHandlers.First().Execute(
                        new Shipment(quoted.GetOriginAddress(), quoted.GetDestinationAddress())
                            {
                                ShipMethodKey = quoted.ShipMethodKey,
                                VersionKey = quoted.VersionKey
                            }
                    )
                : Attempt<IShipment>.Fail(new InvalidOperationException("The configuration Chain Task List could not be instantiated."));

            return attempt;
        }


        /// <summary>
        /// Constructor parameters for the base class activator
        /// </summary>
        private IEnumerable<object> _constructorParameters;
        protected override IEnumerable<object> ConstructorArgumentValues
        {
            get
            {
                return _constructorParameters ?? (_constructorParameters = new List<object>(new object[] { _merchelloContext, _order }));
            }
        }

        /// <summary>
        /// Used for testing
        /// </summary>
        internal int TaskCount
        {
            get { return TaskHandlers.Count(); }
        }
    }
}