namespace Merchello.Web.Models.VirtualContent
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Web.Models.ContentEditing;
    using Merchello.Web.Models.DetachedContent;

    using Umbraco.Core;
    using Umbraco.Core.Models;
    using Umbraco.Core.Models.PublishedContent;
    using Umbraco.Web.Models;

    /// <summary>
    /// The virtual product content.
    /// </summary>
    public class ProductContent : PublishedContentBase, IProductContent
    {
        /// <summary>
        /// The name.
        /// </summary>
        private readonly string _name;

        /// <summary>
        /// The content type.
        /// </summary>
        private readonly PublishedContentType _contentType;

        /// <summary>
        /// The properties.
        /// </summary>
        private readonly IEnumerable<IPublishedProperty> _properties;

        /// <summary>
        /// The sort order.
        /// </summary>
        private readonly int _sortOrder;

        /// <summary>
        /// A value indicating whether or not this is in preview mode.
        /// </summary>
        private readonly bool _isPreviewing;

        /// <summary>
        /// The culture name.
        /// </summary>
        private readonly string _cultureName;

        /// <summary>
        /// The <see cref="ProductDisplay"/>.
        /// </summary>
        private readonly ProductDisplay _display;

        /// <summary>
        /// The _template id.
        /// </summary>
        private Lazy<int> _templateId;

        /// <summary>
        /// The url name.
        /// </summary>
        private Lazy<string> _urlName; 

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductContent"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="contentType">
        /// The content type.
        /// </param>
        /// <param name="display">
        /// The display.
        /// </param>
        /// <param name="properties">
        /// The properties.
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
            string name,
            PublishedContentType contentType,
            ProductDisplay display,
            IEnumerable<IPublishedProperty> properties,
            string cultureName = "en-US",
            int sortOrder = 0,
            bool isPreviewing = false)
        {
            this._name = name;
            this._display = display;
            this._contentType = contentType;
            this._properties = properties;
            this._cultureName = cultureName;
            this._sortOrder = sortOrder;
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
        /// Gets the culture name.
        /// </summary>
        public string CultureName 
        { 
            get
            {
                return _cultureName;
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
        /// Gets the properties.
        /// </summary>
        public override ICollection<IPublishedProperty> Properties
        {
            get
            {
                return this._properties.ToArray(); 
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
                return _templateId.Value;
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
                return _urlName.Value;
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

        /// <summary>
        /// Gets a property by it's alias.
        /// </summary>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <returns>
        /// The <see cref="IPublishedProperty"/>.
        /// </returns>
        public override IPublishedProperty GetProperty(string alias)
        {
            return this._properties.FirstOrDefault(x => x.PropertyTypeAlias.InvariantEquals(alias));
        }
        
        /// <summary>
        /// Initializes the model.
        /// </summary>
        private void Initialize()
        {
            _templateId = new Lazy<int>(() => _display.TemplateId(_cultureName));
        }
    }
}