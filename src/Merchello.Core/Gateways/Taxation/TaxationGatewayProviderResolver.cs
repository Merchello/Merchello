using System;
using System.Collections.Generic;
using Umbraco.Core.ObjectResolution;

namespace Merchello.Core.Gateways.Taxation
{
    /// <summary>
    /// Manages a list of TaxationGatewayProviderBase's
    /// </summary> 
    internal sealed class TaxationGatewayProviderResolver : ManyObjectsResolverBase<TaxationGatewayProviderResolver, TaxationGatewayProviderBase>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaxationGatewayProviderResolver"/> with an intial list of TaxationGatewayProvider types
        /// </summary>
        /// <param name="providers">The list of TaxationGatewayProviders</param>
        internal TaxationGatewayProviderResolver(IEnumerable<Type> providers)
            : base(providers)
        { }


        /// <summary>
        /// Initializes a new instance of the <see cref="TaxationGatewayProviderResolver"/> with an intial list of TaxationGatewayProvider types
        /// </summary>
        /// <param name="providers">The list of TaxationGatewayProviders</param>
        internal TaxationGatewayProviderResolver(params Type[] providers)
            :base (providers)
        { }

        public IEnumerable<TaxationGatewayProviderBase> Providers
        {
            get { return Values; }
        }
    }
}