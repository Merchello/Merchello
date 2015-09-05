namespace Merchello.Web.Models.VirtualContent
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Web.Models.ContentEditing;
    using Merchello.Web.Models.ContentEditing.Content;
    using Merchello.Web.Models.DetachedContent;

    using Umbraco.Core.Models;
    using Umbraco.Core.Models.PublishedContent;

    /// <summary>
    /// The virtual product content.
    /// </summary>
    internal class ProductContent : ProductContentBase, IProductContent
    {
        /// <summary>
        /// The content type.
        /// </summary>
        private readonly PublishedContentType _contentType;

        /// <summary>
        /// The detached content display.
        /// </summary>
        private readonly ProductVariantDetachedContentDisplay _detachedContentDisplay;

        /// <summary>
        /// The sort order.
        /// </summary>
        private readonly int _sortOrder;

        /// <summary>
        /// A value indicating whether or not this is in preview mode.
        /// </summary>
        private readonly bool _isPreviewing;

        /// <summary>
        /// The <see cref="ProductDisplay"/>.
        /// </summary>
        private readonly ProductDisplay _display;


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
        /// <param name="sortOrder">
        /// The sort order.
        /// </param>
        /// <param name="isPreviewing">
        /// The is previewing.
        /// </param>
        public ProductContent(
            PublishedContentType contentType,
            ProductDisplay display,           
            string cultureName = "en-US",
            int sortOrder = 0,
            bool isPreviewing = false)
            : base(display, cultureName)
        {
            this._display = display;
            this._contentType = contentType;
            this._sortOrder = sortOrder;
            this._isPreviewing = isPreviewing;

            this._detachedContentDisplay = _display.DetachedContents.FirstOrDefault(x => x.CultureName == cultureName);
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
        /// Gets the id.
        /// </summary>
        /// <remarks>
        /// Always 0 for virtual content
        /// </remarks>
        public override int Id
        {
            get
            {
                return 0;
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
                return Enumerable.Empty<IProductVariantContent>();
            }
        }

        /// <summary>
        /// Gets the total inventory count.
        /// </summary>
        public int TotalInventoryCount
        {
            get
            {
                return 0;
                //return this.ProductVariants != null
                //           ? this.ProductVariants.Any()
                //                 ? this.ProductVariants.Sum(x => x.TotalInventoryCount)
                //                 : this.CatalogInventories.Sum(x => x.Count)
                //           : 0;
            }
        }

        /// <summary>
        /// Gets the item type.
        /// </summary>
        public override PublishedItemType ItemType
        {
            get
            {
                return PublishedItemType.Content;
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
        /// Gets the children.
        /// </summary>
        /// <remarks>
        /// Always returns empty
        /// </remarks>
        public override IEnumerable<IPublishedContent> Children
        {
            get
            {
                return Enumerable.Empty<IPublishedContent>();
            }
        }

        /// <summary>
        /// Gets the content type.
        /// </summary>
        public override PublishedContentType ContentType
        {
            get
            {
                return this._contentType;
            }
        }

        /// <summary>
        /// Gets the document type alias.
        /// </summary>
        public override string DocumentTypeAlias
        {
            get
            {
                return this._contentType.Alias;
            }
        }

        /// <summary>
        /// Gets the template id.
        /// </summary>
        public override int TemplateId
        {
            get
            {
                return _detachedContentDisplay != null ? _detachedContentDisplay.TemplateId : 0;
            }
        }

        /// <summary>
        /// Gets the sort order.
        /// </summary>
        /// <remarks>
        /// Defaults to 0
        /// </remarks>
        public override int SortOrder
        {
            get
            {
                return this._sortOrder;
            }
        }

        /// <summary>
        /// Gets the url name.
        /// </summary>
        public override string UrlName
        {
            get
            {
                return _detachedContentDisplay != null ? _detachedContentDisplay.Slug : null;
            }
        }

        /// <summary>
        /// Gets the document type id.
        /// </summary>
        public override int DocumentTypeId
        {
            get
            {
                return this._contentType.Id;
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
                return null;
            }
        }

        /// <summary>
        /// Gets the version.
        /// </summary>
        public override Guid Version
        {
            get
            {
                return _display.VersionKey;
            }
        }

        /// <summary>
        /// Gets the level.
        /// </summary>
        /// <remarks>
        /// Always returns 0
        /// </remarks>
        public override int Level
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the writer name.
        /// </summary>
        /// <remarks>
        /// Always returns null
        /// </remarks>
        public override string WriterName
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the creator name.
        /// </summary>
        /// <remarks>
        /// Always returns null
        /// </remarks>
        public override string CreatorName
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the writer id.
        /// </summary>
        /// <remarks>
        /// Always returns 0 (admin)
        /// </remarks>
        public override int WriterId
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the creator id.
        /// </summary>
        /// <remarks>
        /// Always returns 0 (admin)
        /// </remarks>
        public override int CreatorId
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the create date.
        /// </summary>
        /// <remarks>
        /// Always returns DateTime.MinValue
        /// </remarks>
        public override DateTime CreateDate
        {
            get
            {
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// Gets the update date.
        /// </summary>
        /// <remarks>
        /// Always returns DateTime.MinValue
        /// </remarks>
        public override DateTime UpdateDate
        {
            get
            {
                return DateTime.MinValue;
            }
        }
    }
}