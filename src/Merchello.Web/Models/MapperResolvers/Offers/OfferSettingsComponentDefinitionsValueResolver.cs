namespace Merchello.Web.Models.MapperResolvers.Offers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AutoMapper;

    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Web.Models.ContentEditing;

    /// <summary>
    /// The offer settings component definitions value resolver.
    /// </summary>
    public class OfferSettingsComponentDefinitionsValueResolver : ValueResolver<IOfferSettings, IEnumerable<OfferComponentDefinitionDisplay>>
    {
        /// <summary>
        /// The <see cref="IOfferComponentResolver"/>.
        /// </summary>
        /// <remarks>
        /// This is lazy to allow for instantiation of the resolver in the boot manager.
        /// </remarks>
        private readonly Lazy<IOfferComponentResolver> _resolver = new Lazy<IOfferComponentResolver>(() => OfferComponentResolver.Current);

        /// <summary>
        /// The resolve core.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{OfferComponentDefinitionDisplay}"/>.
        /// </returns>
        protected override IEnumerable<OfferComponentDefinitionDisplay> ResolveCore(IOfferSettings source)
        {
            var components = _resolver.Value.GetOfferComponents(source.ComponentDefinitions).Where(x => x != null);
            return components.Select(Mapper.Map<OfferComponentBase, OfferComponentDefinitionDisplay>);
        }
    }
}