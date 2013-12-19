using Merchello.Core.Models.Interfaces;
using Merchello.Core.Services;
using Umbraco.Core.Cache;

namespace Merchello.Core.Gateways
{
    /// <summary>
    /// Defines the GatewayBase
    /// </summary>
    public abstract class GatewayBase : IGateway
    {        
        private readonly IGatewayProvider _gatewayProvider;
        private readonly IGatewayProviderService _gatewayProviderService;

        protected GatewayBase(IGatewayProviderService gatewayProviderService, IGatewayProvider gatewayProvider)
        {
            Mandate.ParameterNotNull(gatewayProviderService, "merchelloContext");
            Mandate.ParameterNotNull(gatewayProvider, "gatewayProvider");

            _gatewayProviderService = gatewayProviderService;
            _gatewayProvider = gatewayProvider;
        }



        /// <summary>
        /// Gets the <see cref="IGatewayProviderService"/>
        /// </summary>
        protected IGatewayProviderService GatewayProviderService
        {
            get { return _gatewayProviderService; }
        }

        /// <summary>
        /// Gets the <see cref="IGatewayProvider"/>
        /// </summary>
        public virtual IGatewayProvider GatewayProvider 
        {
            get { return _gatewayProvider; }
        }
    }
}