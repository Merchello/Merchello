using System;

namespace Merchello.Core.Gateways
{
    /// <summary>
    /// Marker interface for Gateways 
    /// </summary>
    public interface IGateway
    {
        /// <summary>
        /// The unique Key (Guid) for the gateway.  
        /// Used by Merchello in the GatewayProvider's installation/configuration
        /// </summary>
        Guid Key { get; }

        /// <summary>
        /// The Name or title of the gateway
        /// Used by Merchello in the GatewayProvider's installation/configuration
        /// </summary>
        string Name { get; }
    }
}