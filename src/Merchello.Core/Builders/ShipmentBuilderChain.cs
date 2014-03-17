using System.Collections.Generic;
using Merchello.Core.Models;
using Umbraco.Core;

namespace Merchello.Core.Builders
{
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
                return _constructorParameters ?? (_constructorParameters = new List<object>(new object[] { _merchelloContext, _order }));
            }
        }
    }
}