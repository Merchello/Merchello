using System;
using System.Collections.Generic;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Umbraco.Core.Cache;

namespace Merchello.Core.Gateways.Taxation
{
    /// <summary>
    /// Represents the TaxationContext
    /// </summary>
    internal class TaxationContext : GatewayProviderTypedContextBase<TaxationGatewayProviderBase>, ITaxationContext
    {
        public TaxationContext(IGatewayProviderService gatewayProviderService, IRuntimeCacheProvider runtimeCache) 
            : base(gatewayProviderService, runtimeCache)
        { }

        /// <summary>
        /// Resolves all active Taxation Gateway Providers
        /// </summary>
        /// <returns>A collection of all active TypedGatewayProviderinstances</returns>
        public override IEnumerable<TaxationGatewayProviderBase> ResolveAllActiveProviders()
        {
            return GatewayProviderResolver.ResolveByGatewayProviderType<TaxationGatewayProviderBase>(GatewayProviderType.Taxation);
        }

        /// <summary>
        /// Resolves a taxation gateway provider by it's unique key
        /// </summary>
        /// <param name="key"></param>
        /// <returns>A taxation gateway provider</returns>
        public override TaxationGatewayProviderBase ResolveByKey(Guid key)
        {
            return GatewayProviderResolver.ResolveByKey<TaxationGatewayProviderBase>(key);
        }

        public IInvoiceTaxResult CalculateTaxesForInvoice(IInvoice invoice)
        {
            throw new NotImplementedException();
        }
    }
}