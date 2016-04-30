namespace Merchello.Web.Models.MapperResolvers.DetachedContent
{
    using System.Collections.Generic;
    using System.Linq;

    using AutoMapper;

    using Merchello.Core.Models.DetachedContent;
    using Merchello.Core.ValueConverters;

    /// <summary>
    /// Maps the detached data values to an enumerable of KeyValuePair.
    /// </summary>
    internal class DetachedDataValuesResolver : ValueResolver<IDetachedContent, IEnumerable<KeyValuePair<string, string>>>
    {
        /// <summary>
        /// A value indicating whether or not this resolver should resolve values for back office editors.
        /// </summary>
        private readonly bool _isForBackOfficeEditor;

        /// <summary>
        /// Initializes a new instance of the <see cref="DetachedDataValuesResolver"/> class.
        /// </summary>
        /// <param name="isForBackOfficeEditor">
        /// The is for back office editor.
        /// </param>
        public DetachedDataValuesResolver(bool isForBackOfficeEditor = false)
        {
            _isForBackOfficeEditor = isForBackOfficeEditor;
        }

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
            if (source.DetachedDataValues == null) return Enumerable.Empty<KeyValuePair<string, string>>();

            return source.DetachedDataValues.AsEnumerable();
        }
    }
}