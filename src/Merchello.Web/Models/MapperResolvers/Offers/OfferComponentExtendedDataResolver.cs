namespace Merchello.Web.Models.MapperResolvers.Offers
{
    using System.Collections.Generic;
    using System.Linq;

    using AutoMapper;

    using Merchello.Core.Marketing.Offer;

    /// <summary>
    /// The offer component extended data resolver.
    /// </summary>
    public class OfferComponentExtendedDataResolver : ValueResolver<OfferComponentBase, IEnumerable<KeyValuePair<string, string>>>
    {
        /// <summary>
        /// The resolve core.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        ///  A collection of key value pairs that represent an <see cref="Core.Models.ExtendedDataCollection"/>.
        /// </returns>
        protected override IEnumerable<KeyValuePair<string, string>> ResolveCore(OfferComponentBase source)
        {
            return source.OfferComponentDefinition.ExtendedData.AsEnumerable();
        }
    }
}