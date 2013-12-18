using Merchello.Core.Models.Interfaces;
using Umbraco.Core.Cache;

namespace Merchello.Core.Gateways
{
    /// <summary>
    /// Defines the GatewayBase
    /// </summary>
    public abstract class GatewayBase : IGateway
    {        
        private readonly IGatewayProvider _gatewayProvider;
        private readonly IMerchelloContext _merchelloContext;

        protected GatewayBase(IMerchelloContext merchelloContext, IGatewayProvider gatewayProvider)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            Mandate.ParameterNotNull(gatewayProvider, "gatewayProvider");

            _merchelloContext = merchelloContext;
            _gatewayProvider = gatewayProvider;
        }



        /// <summary>
        /// Gets the <see cref="IMerchelloContext"/>
        /// </summary>
        protected IMerchelloContext MerchelloContext
        {
            get { return _merchelloContext; }
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