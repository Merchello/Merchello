using System;
using System.Collections.Generic;
using Merchello.Core.Gateways.Taxation;
using Umbraco.Core.ObjectResolution;

namespace Merchello.Core.Gateways.Shipping
{
    internal sealed class ShippingGatewayProviderResolver : LazyManyObjectsResolverBase<ShippingGatewayProviderResolver, ShippingGatewayProviderBase>
    {
         /// <summary>
        /// Initializes a new instance of the <see cref="ShippingGatewayProviderResolver"/> with an intial list of ShippingGatewayProvider types
        /// </summary>
        /// <param name="providers">The list of ShippingGatewayProviders</param>
        internal ShippingGatewayProviderResolver(Func<IEnumerable<Type>> providers)
            : base(providers)
        { }


        public IEnumerable<Type> ProviderTypes
        {
            get { return InstanceTypes; }
        }
    }
}