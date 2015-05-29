namespace Merchello.Web.Models.MapperResolvers.Offers
{
    using AutoMapper;

    using Merchello.Core.Marketing.Offer;
    using Merchello.Web.Models.ContentEditing;
    using Merchello.Web.Trees;

    using Umbraco.Core;

    /// <summary>
    /// The offer provider back office attribute value resolver.
    /// </summary>
    internal class OfferProviderBackOfficeAttributeValueResolver : ValueResolver<IOfferProvider, BackOfficeTreeDisplay>
    {
        /// <summary>
        /// The resolve core.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        protected override BackOfficeTreeDisplay ResolveCore(IOfferProvider source)
        {
            var att = source.GetType().GetCustomAttribute<BackOfficeTreeAttribute>(false);
            if (att == null) return null;


            return new BackOfficeTreeDisplay()
                       {
                           RouteId = att.RouteId,
                           ParentRouteId = att.ParentRouteId,
                           Icon = att.Icon,
                           RoutePath = att.RoutePath,
                           Title = att.Title,
                           SortOrder = att.SortOrder
                       };
        }
    }
}