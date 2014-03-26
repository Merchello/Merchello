using System;
using System.Collections.Generic;
using Umbraco.Core.ObjectResolution;

namespace Merchello.Core.Gateways.Payment
{
    internal sealed class PaymentGatewayProviderResolver : LazyManyObjectsResolverBase<PaymentGatewayProviderResolver, PaymentGatewayProviderBase>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentGatewayProviderResolver"/> with an intial list of PaymentGatewayProvider types
        /// </summary>
        /// <param name="providers">The list of PaymentGatewayProviders</param>
        internal PaymentGatewayProviderResolver(Func<IEnumerable<Type>> providers)
            : base(providers)
        { }

        public IEnumerable<Type> ProviderTypes
        {
            get { return InstanceTypes; }
        }

    }
}