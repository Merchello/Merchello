namespace Merchello.Web.Models.VirtualContent
{
    using System.Linq;

    using Merchello.Core.Configuration;
    using Merchello.Web.Models.ContentEditing;

    using Umbraco.Core.Events;
    using Umbraco.Core.Models;
    using Umbraco.Core.Models.PublishedContent;

    /// <summary>
    /// Represents a ProductContentFactory.
    /// </summary>
    public class ProductContentFactory : IProductContentFactory
    {
        /// <summary>
        /// The parent.
        /// </summary>
        private IPublishedContent _parent;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductContentFactory"/> class.
        /// </summary>
        public ProductContentFactory()
        {
            this.Initialize();
        }

        /// <summary>
        /// The initializing.
        /// </summary>
        public static event TypedEventHandler<ProductContentFactory, VirtualContentEventArgs> Initializing; 

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

            var slugPrefix = MerchelloConfiguration.Current.GetProductSlugCulturePrefix(cultureName);

            var publishedContentType = PublishedContentType.Get(PublishedItemType.Content, detachedContent.DetachedContentType.UmbContentType.Alias);

            return new ProductContent(publishedContentType, display, cultureName, _parent, slugPrefix);
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        private void Initialize()
        {
            var args = new VirtualContentEventArgs(_parent);
            Initializing.RaiseEvent(args, this);
            _parent = args.Parent;
        }
    }
}