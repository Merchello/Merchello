namespace Merchello.Web.Models.MapperResolvers.DetachedContent
{
    using System.Collections.Generic;
    using System.Linq;

    using AutoMapper;

    using Umbraco.Core.Models;

    /// <summary>
    /// The embedded content tabs resolver.
    /// </summary>
    internal class EmbeddedContentTabsResolver : ValueResolver<IContentType, IEnumerable<string>>
    {
        /// <summary>
        /// The resolve core.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{String}"/>.
        /// </returns>
        protected override IEnumerable<string> ResolveCore(IContentType source)
        {
            return source.CompositionPropertyGroups.Select(y => y.Name).Distinct();
        }
    }
}