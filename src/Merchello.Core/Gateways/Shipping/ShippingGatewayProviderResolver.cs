using System;
using System.Collections.Generic;
using Merchello.Core.Gateways.Taxation;
using Umbraco.Core.ObjectResolution;

namespace Merchello.Core.Gateways.Shipping
{
    internal sealed class ShippingGatewayProviderResolver : ManyObjectsResolverBase<ShippingGatewayProviderResolver, ShippingGatewayProviderBase>
    {
         /// <summary>
        /// Initializes a new instance of the <see cref="ShippingGatewayProviderResolver"/> with an intial list of ShippingGatewayProvider types
        /// </summary>
        /// <param name="providers">The list of ShippingGatewayProviders</param>
        internal ShippingGatewayProviderResolver(IEnumerable<Type> providers)
            : base(providers)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShippingGatewayProviderResolver"/> with an intial list of ShippingGatewayProvider types
        /// </summary>
        /// <param name="providers">The list of ShippingGatewayProviders</param>
        internal ShippingGatewayProviderResolver(params Type[] providers)
            :base (providers)
        { }

        public IEnumerable<ShippingGatewayProviderBase> Providers
        {
            get { return Values; }
        }
    }
}