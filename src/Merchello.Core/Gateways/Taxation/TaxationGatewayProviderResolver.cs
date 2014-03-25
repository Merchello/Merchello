using System;
using System.Collections.Generic;
using Umbraco.Core.ObjectResolution;

namespace Merchello.Core.Gateways.Taxation
{
    /// <summary>
    /// Manages a list of ITaxationGatewayProvider's
    /// </summary> 
    internal sealed class TaxationGatewayProviderResolver : ManyObjectsResolverBase<TaxationGatewayProviderResolver, ITaxationGatewayProvider>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaxationGatewayProviderResolver"/>
        /// </summary>
        /// <param name="providers"></param>
        internal TaxationGatewayProviderResolver(IEnumerable<Type> providers)
            : base(providers)
        { }


        internal TaxationGatewayProviderResolver(params Type[] providers)
        { }
    }
}