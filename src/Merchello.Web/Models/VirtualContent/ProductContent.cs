namespace Merchello.Web.Models.VirtualContent
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Web.Models.ContentEditing;

    using Umbraco.Core.Models;
    using Umbraco.Core.Models.PublishedContent;

    /// <summary>
    /// The virtual product content.
    /// </summary>
    internal class ProductContent : ProductContentBase, IProductContent
    {
        /// <summary>
        /// A value indicating whether or not this is in preview mode.
        /// </summary>
        private readonly bool _isPreviewing;

        /// <summary>
        /// The <see cref="ProductDisplay"/>.
        /// </summary>
        private readonly ProductDisplay _display;

        /// <summary>
        /// The variant content.
        /// </summary>
        private Lazy<IEnumerable<IProductVariantContent>> _variantContent; 

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductContent"/> class.
        /// </summary>
        /// <param name="contentType">
        /// The content type.
        /// </param>
        /// <param name="display">
        /// The display.
        /// </param>
        /// <param name="cultureName">
        /// The culture name
        /// </param>
        /// <param name="isPreviewing">
        /// The is previewing.
        /// </param>
        public ProductContent(
            PublishedContentType contentType,
            ProductDisplay display,           
            string cultureName = "en-US",
            bool isPreviewing = false)
            : base(display, contentType, cultureName)
        {
            this._display = display;
            this._isPreviewing = isPreviewing;

            this.Initialize();
        }

        /// <summary>
        /// Gets the key.
        /// </summary>
        public Guid Key
        {
            get
            {
                return _display.Key;
            }
        }

        /// <summary>
        /// Gets the product variant key.
        /// </summary>
        public Guid ProductVariantKey
        {
            get
            {
                return _display.ProductVariantKey;
            }
        }      

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <remarks>
        /// Always returns null
        /// </remarks>
        public override string Path
        {
            get
            {
                // this may need to be set to the root id so that it can pass the security check
                return "-1,0";
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return this._display.Name;
            }
        }

        /// <summary>
        /// Gets the product options.
        /// </summary>
        public IEnumerable<ProductOptionDisplay> ProductOptions
        {
            get
            {
                return _display.ProductOptions;
            }
        }

        /// <summary>
        /// Gets the product variants.
        /// </summary>
        public IEnumerable<IProductVariantContent> ProductVariants
        {
            get
            {
                return _variantContent.Value;
            }
        }

        /// <summary>
        /// Gets the total inventory count.
        /// </summary>
        public new int TotalInventoryCount
        {
            get
            {
                // use the display object here so the Lazy does not execute if not needed
                return _display.ProductVariants != null
                           ? _display.ProductVariants.Any()
                                 ? _display.ProductVariants.Sum(x => x.TotalInventoryCount)
                                 : this.CatalogInventories.Sum(x => x.Count)
                           : 0;
            }
        }      

        /// <summary>
        /// Gets a value indicating whether is a draft.
        /// </summary>
        public override bool IsDraft
        {
            get { return this._isPreviewing; }
        }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <remarks>
        /// Always returns null
        /// </remarks>
        public override IPublishedContent Parent
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the URL.
        /// </summary>
        public override string Url
        {
            get
            {
                return UrlName.EnsureStartsAndEndsWith('/');
            }
        }

        /// <summary>
        /// Gets the template id.
        /// </summary>
        public override int TemplateId
        {
            get
            {
                return DetachedContentDisplay != null ? DetachedContentDisplay.TemplateId : 0;
            }
        }

        /// <summary>
        /// Initializes the model.
        /// </summary>
        private void Initialize()
        {
            _variantContent = new Lazy<IEnumerable<IProductVariantContent>>(() => _display.ProductVariantsAsProductVariantContent(CultureName));
        }
    }
}