using System.Collections.Generic;
using Merchello.Core.Models;
using Umbraco.Core;

namespace Merchello.Core.Builders
{
    internal sealed class ShipmentBuilderChain : BuildChainBase<IShipment>
    {
        private readonly IOrder _order;

        public ShipmentBuilderChain(IOrder order)
        {
            Mandate.ParameterNotNull(order, "order");

            _order = order;
        }

        /// <summary>
        /// Builds the order
        /// </summary>
        /// <returns>Attempt{IShipment}</returns>
        public override Attempt<IShipment> Build()
        {
            throw new System.NotImplementedException();
        }


        /// <summary>
        /// Constructor parameters for the base class activator
        /// </summary>
        private IEnumerable<object> _constructorParameters;
        protected override IEnumerable<object> ConstructorArgumentValues
        {
            get
            {
                return _constructorParameters ?? (_constructorParameters = new List<object>(new object[] { _order }));
            }
        }
    }
}