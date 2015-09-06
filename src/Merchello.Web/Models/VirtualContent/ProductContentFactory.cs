namespace Merchello.Web.Models.VirtualContent
{
    using System.Linq;

    using Merchello.Web.Models.ContentEditing;

    using Umbraco.Core;
    using Umbraco.Core.Models;
    using Umbraco.Core.Models.PublishedContent;
    using Umbraco.Core.Services;

    /// <summary>
    /// Represents a ProductContentFactory.
    /// </summary>
    internal class ProductContentFactory : IProductContentFactory
    {
        /// <summary>
        /// The <see cref="IContentTypeService"/>.
        /// </summary>
        private readonly IContentTypeService _contentTypeService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductContentFactory"/> class.
        /// </summary>
        public ProductContentFactory()
            : this(ApplicationContext.Current.Services.ContentTypeService)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductContentFactory"/> class.
        /// </summary>
        /// <param name="contentTypeService">
        /// The content type service.
        /// </param>
        public ProductContentFactory(IContentTypeService contentTypeService)
        {
            Mandate.ParameterNotNull(contentTypeService, "contentTypeService");
            _contentTypeService = contentTypeService;
        }

        /// <summary>
        /// The build content.
        /// </summary>
        /// <param name="display">
        /// The display.
        /// </param>
        /// <param name="cultureName">
        /// The culture name.
        /// </param>
        /// <returns>
        /// The <see cref="IProductContent"/>.
        /// </returns>
        public IProductContent BuildContent(ProductDisplay display, string cultureName)
        {
            var detachedContent = display.DetachedContents.FirstOrDefault(x => x.CultureName == cultureName);
            if (detachedContent == null && cultureName == Core.Constants.DefaultCultureName)
            {
                detachedContent =
                    display.DetachedContents.FirstOrDefault(x => x.CultureName == Core.Constants.DefaultCultureName);
            }

            if (detachedContent == null) return null;

            var publishedContentType = PublishedContentType.Get(PublishedItemType.Content, detachedContent.DetachedContentType.UmbContentType.Alias);

            return new ProductContent(publishedContentType, display, cultureName);
        }

        public IProductVariantContent BuildContent(ProductVariantDisplay display, string cultureName)
        {
            throw new System.NotImplementedException();
        }
    }
}