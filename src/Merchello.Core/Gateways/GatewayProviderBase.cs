using System;
using Merchello.Core.Models;
using Merchello.Core.Services;

namespace Merchello.Core.Gateways
{
    /// <summary>
    /// Defines the GatewayBase
    /// </summary>
    public abstract class GatewayProviderBase : IGateway
    {        
        private readonly IGatewayProvider _gatewayProvider;
        private readonly IGatewayProviderService _gatewayProviderService;

        protected GatewayProviderBase(IGatewayProviderService gatewayProviderService, IGatewayProvider gatewayProvider)
        {
            Mandate.ParameterNotNull(gatewayProviderService, "gatewayProviderService");
            Mandate.ParameterNotNull(gatewayProvider, "gatewayProvider");

            _gatewayProviderService = gatewayProviderService;
            _gatewayProvider = gatewayProvider;
        }

        /// <summary>
        /// The name of the GatewayProvider
        /// </summary>
        protected abstract string Name { get; }

        /// <summary>
        /// The unique Key that will be used
        /// </summary>
        public abstract Guid Key { get;  }

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