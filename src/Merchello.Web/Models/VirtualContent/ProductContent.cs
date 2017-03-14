namespace Merchello.Web.Models.VirtualContent
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Configuration;
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
        /// The parent node.
        /// </summary>
        private readonly IPublishedContent _parent;

        /// <summary>
        /// The variant content.
        /// </summary>
        private Lazy<IEnumerable<IProductVariantContent>> _variantContent;

        /// <summary>
        /// The options collection.
        /// </summary>
        private IEnumerable<IProductOptionWrapper> _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductContent"/> class.
        /// </summary>
        /// <param name="contentType">
        /// The content type.
        /// </param>
        /// <param name="optionContentTypes">
        /// All of the possible content types for option attribute content
        /// </param>
        /// <param name="display">
        /// The display.
        /// </param>
        /// <param name="parent">
        /// The parent <see cref="IPublishedContent"/>
        /// </param>
        /// <param name="specificCulture">
        /// The specific culture name
        /// </param>
        /// <param name="isPreviewing">
        /// The is previewing.
        /// </param>
        public ProductContent(
            PublishedContentType contentType,
            IDictionary<Guid, PublishedContentType> optionContentTypes,
            ProductDisplay display,           
            IPublishedContent parent = null,
            string specificCulture = "en-US",
            bool isPreviewing = false)
            : base(display, contentType, optionContentTypes, specificCulture)
        {
            this._display = display;
            this._parent = parent;
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
                return _parent == null ? "-1,0" : _parent.Path + ",0";
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
        [Obsolete("Use Options property")]
        public IEnumerable<ProductOptionDisplay> ProductOptions
        {
            get
            {
                return _display.ProductOptions.OrderBy(x => x.SortOrder);
            }
        }

        /// <summary>
        /// Gets the options.
        /// </summary>
        public IEnumerable<IProductOptionWrapper> Options
        {
            get
            {
                return _options.OrderBy(x => x.SortOrder);
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
                return ProductVariants != null
                           ? ProductVariants.Any()
                                 ? ProductVariants.Sum(x => x.TotalInventoryCount)
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
                return _parent;
            }
        }

        /// <summary>
        /// Gets the URL.
        /// </summary>
        public override string Url
        {
            get
            {
                return SlugPrefix.IsNullOrWhiteSpace()
                           ? UrlName.EnsureStartsAndEndsWith('/')
                           : string.Format("{0}{1}", SlugPrefix.EnsureEndsWith('/'), UrlName)
                                 .EnsureStartsAndEndsWith('/');
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
        /// Gets the update date.
        /// </summary>
        public override DateTime UpdateDate
        {
            get
            {
                return _display.UpdateDate;
            }
        }

        /// <summary>
        /// Gets the product display.
        /// </summary>
        internal ProductDisplay ProductDisplay
        {
            get
            {
                return _display;
            }
        }

        /// <summary>
        /// Gets the slug prefix.
        /// </summary>
        private string SlugPrefix
        {
            get
            {
                return MerchelloConfiguration.Current.GetProductSlugCulturePrefix(CultureName);
            }
        }

        /// <summary>
        /// The specify culture.
        /// </summary>
        /// <param name="cultureName">
        /// The culture name.
        /// </param>
        public void SpecifyCulture(string cultureName)
        {
            if (!_display.DetachedContents.Any(x => x.CultureName == cultureName && x.CanBeRendered)) return;
            
            this.ChangeCulture(cultureName);
            foreach (var variant in ProductVariants)
            {
                ((ProductContentBase)variant).ChangeCulture(cultureName);
            }                            
        }

        /// <summary>
        /// Initializes the model.
        /// </summary>
        private void Initialize()
        {
            _options = _display.ProductOptions.Select(x => x.ProductOptionAsProductOptionWrapper(this, this.OptionContentTypes)).ToArray();
            _variantContent = new Lazy<IEnumerable<IProductVariantContent>>(() => _display.ProductVariantsAsProductVariantContent(this.OptionContentTypes, _options, CultureName, this));
        }
    }
}