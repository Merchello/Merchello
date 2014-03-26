using System;
using System.Collections.Generic;
using Merchello.Core.Gateways.Shipping;
using Umbraco.Core.ObjectResolution;

namespace Merchello.Core.Gateways.Payment
{
    internal sealed class PaymentGatewayProviderResolver : ManyObjectsResolverBase<PaymentGatewayProviderResolver, PaymentGatewayProviderBase>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentGatewayProviderResolver"/> with an intial list of PaymentGatewayProvider types
        /// </summary>
        /// <param name="providers">The list of PaymentGatewayProviders</param>
        internal PaymentGatewayProviderResolver(IEnumerable<Type> providers)
            : base(providers)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentGatewayProviderResolver"/> with an intial list of PaymentGatewayProvider types
        /// </summary>
        /// <param name="providers">The list of PaymentGatewayProviders</param>
        internal PaymentGatewayProviderResolver(params Type[] providers)
            :base (providers)
        { }

        public IEnumerable<PaymentGatewayProviderBase> Providers
        {
            get { return Values; }
        }
    }
}