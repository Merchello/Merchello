namespace Merchello.Web.Models.MapperResolvers.DetachedContent
{
    using System.Linq;

    using AutoMapper;

    using Merchello.Core.Models.DetachedContent;
    using Merchello.Web.Models.ContentEditing.Content;

    using umbraco;

    using Umbraco.Core;
    using Umbraco.Core.Services;

    /// <summary>
    /// Resolves <see cref="UmbContentTypeDisplay"/> for <see cref="IDetachedContentType"/> mappings.
    /// </summary>
    public class UmbContentTypeResolver : ValueResolver<IDetachedContentType, UmbContentTypeDisplay>
    {
        /// <summary>
        /// The <see cref="IContentTypeService"/>.
        /// </summary>
        private readonly IContentTypeService _contentTypeService = ApplicationContext.Current.Services.ContentTypeService;

        /// <summary>
        /// Resolves <see cref="UmbContentTypeDisplay"/> for <see cref="IDetachedContentType"/> mappings.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="UmbContentTypeDisplay"/>.
        /// </returns>
        protected override UmbContentTypeDisplay ResolveCore(IDetachedContentType source)
        {
            var contentType = source.ContentTypeKey != null ?
                 _contentTypeService.GetContentType(source.ContentTypeKey.Value) :
                 null;

            return contentType != null ? contentType.ToUmbContentTypeDisplay() : new UmbContentTypeDisplay() { AllowedTemplates = Enumerable.Empty<UmbTemplateDisplay>() };
        }
    }
}