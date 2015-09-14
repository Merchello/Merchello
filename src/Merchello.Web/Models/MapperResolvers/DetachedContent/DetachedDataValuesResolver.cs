namespace Merchello.Web.Models.MapperResolvers.DetachedContent
{
    using System.Collections.Generic;
    using System.Linq;

    using AutoMapper;

    using Merchello.Core.Models.DetachedContent;

    /// <summary>
    /// Maps the detached data values to an enumerable of KeyValuePair.
    /// </summary>
    internal class DetachedDataValuesResolver : ValueResolver<IDetachedContent, IEnumerable<KeyValuePair<string, string>>>
    {
        /// <summary>
        /// Performs the work of mapping the value.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{KeyValuePair}"/>.
        /// </returns>
        protected override IEnumerable<KeyValuePair<string, string>> ResolveCore(IDetachedContent source)
        {
            return source.DetachedDataValues != null
                       ? source.DetachedDataValues.AsEnumerable()
                       : Enumerable.Empty<KeyValuePair<string, string>>();
        }
    }
}