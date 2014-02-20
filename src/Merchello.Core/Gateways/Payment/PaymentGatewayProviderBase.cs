using System;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Umbraco.Core.Cache;

namespace Merchello.Core.Gateways.Payment
{
    public abstract class PaymentGatewayProviderBase  : GatewayProviderBase, IPaymentGatewayProvider
    {
        protected PaymentGatewayProviderBase(IGatewayProviderService gatewayProviderService, IGatewayProvider gatewayProvider, IRuntimeCacheProvider runtimeCacheProvider) 
            : base(gatewayProviderService, gatewayProvider, runtimeCacheProvider)
        { }


    }
}