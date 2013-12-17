using Merchello.Core.Models.Interfaces;

namespace Merchello.Core.Gateways
{
    /// <summary>
    /// Defines the GatewayBase
    /// </summary>
    public abstract class GatewayBase : IGatewayBase
    {
        private readonly IGatewayProvider _gatewayProvider;

        protected GatewayBase(IGatewayProvider gatewayProvider)
        {
            Mandate.ParameterNotNull(gatewayProvider, "gatewayProvider");

            _gatewayProvider = gatewayProvider;
        }

        /// <summary>
        /// Gets the <see cref="IGatewayProvider"/>
        /// </summary>
        protected virtual IGatewayProvider GatewayProvider 
        {
            get { return _gatewayProvider; }
        }
    }
}