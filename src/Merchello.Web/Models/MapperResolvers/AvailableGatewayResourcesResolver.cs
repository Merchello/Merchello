namespace Merchello.Web.Models.MapperResolvers
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using AutoMapper;

    using Merchello.Core.Gateways;
    using Merchello.Web.Models.ContentEditing;

    /// <summary>
    /// Maps the ListResourcesOffered to an enumeration of <see cref="GatewayProviderDisplay"/>.
    /// </summary>
    internal class AvailableGatewayResourcesResolver : ValueResolver<GatewayProviderBase, IEnumerable<GatewayResourceDisplay>>
    {
        /// <summary>
        /// The resolve core.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{GatewayResourceDisplay}"/>.
        /// </returns>
        protected override IEnumerable<GatewayResourceDisplay> ResolveCore(GatewayProviderBase source)
        {
            return source.ListResourcesOffered().Select(x => x.ToGatewayResourceDisplay());
        }
    }
}