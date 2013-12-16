using System.Collections.Generic;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Services;

namespace Merchello.Web.Shipping
{
    public abstract class ShippingGatewayProviderBase : GatewayProviderBase, IShippingGatewayProvider
    {
        private readonly IShippingService _shippingService;

        protected ShippingGatewayProviderBase()
            : this(MerchelloContext.Current)
        { }

        internal ShippingGatewayProviderBase(IMerchelloContext merchelloContext)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");

            _shippingService = merchelloContext.Services.ShippingService;
        }

        /// <summary>
        /// Returns a collection of all possible ship methods associated with this provider
        /// </summary>
        /// <remarks>
        /// Dictionary items
        /// 
        /// Key : service code
        /// Value : Ship method name
        ///  
        /// </remarks>
        /// <returns></returns>
        public abstract IDictionary<string, string> GetAllShipMethods();
    
        /// <summary>
        /// Returns a collection of ship methods assigned for this specific provider configuration (associated with the ShipCountry)
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IGatewayShipMethod> GetActiveShipMethods(IShipCountry shipCountry)
        {
            throw new System.NotImplementedException();
        }
    }
}