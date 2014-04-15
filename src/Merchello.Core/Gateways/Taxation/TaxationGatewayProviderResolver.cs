using System;
using System.Collections.Generic;
using Umbraco.Core.ObjectResolution;

namespace Merchello.Core.Gateways.Taxation
{
    /// <summary>
    /// Manages a list of TaxationGatewayProviderBase's
    /// </summary> 
    internal sealed class TaxationGatewayProviderResolver : LazyManyObjectsResolverBase<TaxationGatewayProviderResolver, TaxationGatewayProviderBase>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaxationGatewayProviderResolver"/> with an intial list of TaxationGatewayProvider types
        /// </summary>
        /// <param name="providers">The list of TaxationGatewayProviders</param>
        internal TaxationGatewayProviderResolver(Func<IEnumerable<Type>> providers)
            : base(providers)
        { }



        public IEnumerable<Type> ProviderTypes
        {
            get { return InstanceTypes; }
        }
    }
}