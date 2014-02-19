using System;
using System.Collections.Generic;
using Merchello.Core.Services;
using Umbraco.Core.Cache;

namespace Merchello.Core.Gateways.Payment
{
    /// <summary>
    /// Defines the Payment Context
    /// </summary>
    public interface IPaymentContext : IGatewayProviderTypedContextBase<PaymentGatewayProviderBase>
    { }

    /// <summary>
    /// Represents the Payment Context
    /// </summary>
    internal class PaymentContext : GatewayProviderTypedContextBase<PaymentGatewayProviderBase>, IPaymentContext
    {
        public PaymentContext(IGatewayProviderService gatewayProviderService, IRuntimeCacheProvider runtimeCache) 
            : base(gatewayProviderService, runtimeCache)
        { }


        /// <summary>
        /// Resolves all active payment gateway providers
        /// </summary>
        /// <returns>A collection of all active payment gateway providers</returns>
        public override IEnumerable<PaymentGatewayProviderBase> ResolveAllActiveProviders()
        {
            return GatewayProviderResolver.ResolveByGatewayProviderType<PaymentGatewayProviderBase>(GatewayProviderType.Shipping);
        }

        /// <summary>
        /// Resolves a payment gateway provider by it's unique key
        /// </summary>
        /// <param name="key"></param>
        /// <returns>A payment gateway provider</returns>
        public override PaymentGatewayProviderBase ResolveByKey(Guid key)
        {
            return GatewayProviderResolver.ResolveByKey<PaymentGatewayProviderBase>(key);
        }
    }
}