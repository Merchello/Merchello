using System;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Models;

namespace Merchello.Core.Gateways
{
    /// <summary>
    /// Represents the GatewayContext.  Provides access to <see cref="IGatewayProvider"/>s
    /// </summary>
    internal class GatewayContext : IGatewayContext
    {
        
        internal GatewayContext(IShippingGatewayContext shippingGateways)
        {
            Mandate.ParameterNotNull(shippingGateways, "shippingGateways");

            _shippingGateway = shippingGateways;
        }

        private readonly IShippingGatewayContext _shippingGateway;
        public IShippingGatewayContext Shipping
        {
            get
            {
                if(_shippingGateway == null) throw new InvalidOperationException("The ShippingGatewayContext is not set in the GatewayContext");

                return _shippingGateway;
            }

        }
    }
}