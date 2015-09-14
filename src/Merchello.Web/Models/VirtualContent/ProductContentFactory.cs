namespace Merchello.Web.Models.VirtualContent
{
    using System.Linq;

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
        /// <returns>
        /// The <see cref="IProductContent"/>.
        /// </returns>
        public IProductContent BuildContent(ProductDisplay display)
        {
            if (!display.DetachedContents.Any(x => x.CanBeRendered)) return null;

            // assert there is at least one the can be rendered
            var detachedContent = display.DetachedContents.FirstOrDefault(x => x.CanBeRendered);
            
            if (detachedContent == null) return null;

            var publishedContentType = PublishedContentType.Get(PublishedItemType.Content, detachedContent.DetachedContentType.UmbContentType.Alias);
            
            return new ProductContent(publishedContentType, display, _parent);
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