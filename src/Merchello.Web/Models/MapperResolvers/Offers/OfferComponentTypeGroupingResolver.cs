namespace Merchello.Web.Models.MapperResolvers.Offers
{
    using AutoMapper;

    using Merchello.Core.Marketing.Offer;

    /// <summary>
    /// The offer component type grouping resolver.
    /// </summary>
    public class OfferComponentTypeGroupingResolver : ValueResolver<OfferComponentBase, string>
    {
        /// <summary>
        /// Maps the TypeGrouping Type full name.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected override string ResolveCore(OfferComponentBase source)
        {
            return source.TypeGrouping.FullName;
        }
    }
}