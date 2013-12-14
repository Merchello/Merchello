using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Services;

namespace Merchello.Web.Shipping
{
    public class ShippingGatewayProviderBase : GatewayProviderBase, IShippingGatewayProvider
    {
        private readonly IShippingService _shippingService;

        protected ShippingGatewayProviderBase()
            : this(MerchelloContext.Current.Services.ShippingService)
        { }

        internal ShippingGatewayProviderBase(IShippingService shippingService)
        {
            Mandate.ParameterNotNull(shippingService, "shippingService");

            _shippingService = shippingService;
        }
    }
}