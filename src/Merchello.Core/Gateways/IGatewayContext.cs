using System.Collections.Generic;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Models;

namespace Merchello.Core.Gateways
{
    /// <summary>
    /// Defines the GatewayContext
    /// </summary>
    public interface IGatewayContext
    {


        /// <summary>
        /// Gets the <see cref="IShippingGatewayContext"/>
        /// </summary>
        IShippingGatewayContext Shipping { get; }
    }
}