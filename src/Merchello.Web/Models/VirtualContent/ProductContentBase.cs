namespace Merchello.Web.Models.VirtualContent
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Web.Models.ContentEditing;
    using Merchello.Web.Models.ContentEditing.Content;

    using Umbraco.Core;
    using Umbraco.Core.Models;
    using Umbraco.Core.Models.PublishedContent;
    using Umbraco.Web.Models;

    /// <summary>
    /// Base class for Product content classes
    /// </summary>
    public abstract class ProductContentBase : PublishedContentBase, IProductContentBase
    {
        /// <summary>
        /// The content type.
        /// </summary>
        private readonly PublishedContentType _contentType;

        /// <summary>
        /// The product base.
        /// </summary>
        private readonly ProductDisplayBase _productBase;

        /// <summary>
        /// The detached content display.
        /// </summary>
        private ProductVariantDetachedContentDisplay _detachedContentDisplay;

        /// <summary>
        /// The properties.
        /// </summary>
        private Lazy<Dictionary<string, IEnumerable<IPublishedProperty>>> _properties;


        /// <summary>
        /// Initializes a new instance of the <see cref="ProductContentBase"/> class.
        /// </summary>
        /// <param name="productBase">
        /// The product base.
        /// </param>
        /// <param name="contentType">
        /// The content Type.
        /// </param>
        /// <param name="optionContentTypes">
        /// The option Content Types.
        /// </param>
        /// <param name="specificCulture">
        /// Specifically sets the culture
        /// </param>
        protected ProductContentBase(ProductDisplayBase productBase, PublishedContentType contentType, IDictionary<Guid, PublishedContentType> optionContentTypes, string specificCulture)
        {
            Mandate.ParameterNotNull(productBase, "productBase");
            _productBase = productBase;
            this.CultureName = specificCulture;
            _contentType = contentType;
            this.OptionContentTypes = optionContentTypes;
            this.Initialize();
        }

        /// <summary>
        /// Gets the culture name.
        /// </summary>
        public string CultureName { get; private set; }

        /// <summary>
        /// Gets the price.
        /// </summary>
        public decimal Price
        {
            get
            {
                return _productBase.Price;
            }
        }

        /// <summary>
        /// Gets the sale price.
        /// </summary>
        public decimal SalePrice
        {
            get
            {
                return _productBase.SalePrice;
            }
        }

        /// <summary>
        /// Gets a value indicating whether on sale.
        /// </summary>
        public bool OnSale
        {
            get
            {
                return _productBase.OnSale;
            }
        }

        /// <summary>
        /// Gets a value indicating whether available.
        /// </summary>
        public bool Available
        {
            get
            {
                return _productBase.Available;
            }
        }

        /// <summary>
        /// Gets a value indicating whether track inventory.
        /// </summary>
        public bool TrackInventory
        {
            get
            {
                return _productBase.TrackInventory;
            }
        }

        /// <summary>
        /// Gets a value indicating whether shippable.
        /// </summary>
        public bool Shippable
        {
            get
            {
                return _productBase.Shippable;
            }
        }

        /// <summary>
        /// Gets a value indicating whether taxable.
        /// </summary>
        public bool Taxable
        {
            get
            {
                return _productBase.Taxable;
            }
        }

        /// <summary>
        /// Gets the SKU.
        /// </summary>
        public string Sku
        {
            get
            {
                return _productBase.Sku;
            }
        }

        /// <summary>
        /// Gets the cost of goods.
        /// </summary>
        public decimal CostOfGoods
        {
            get
            {
                return _productBase.CostOfGoods;
            }
        }

        /// <summary>
        /// Gets the manufacturer.
        /// </summary>
        public string Manufacturer
        {
            get
            {
                return _productBase.Manufacturer;
            }
        }

        /// <summary>
        /// Gets the manufacturer model number.
        /// </summary>
        public string ManufacturerModelNumber
        {
            get
            {
                return _productBase.ManufacturerModelNumber;
            }
        }

        /// <summary>
        /// Gets the weight.
        /// </summary>
        public decimal Weight
        {
            get
            {
                return _productBase.Weight;
            }
        }

        /// <summary>
        /// Gets the length.
        /// </summary>
        public decimal Length
        {
            get
            {
                return _productBase.Length;
            }
        }

        /// <summary>
        /// Gets the width.
        /// </summary>
        public decimal Width
        {
            get
            {
                return _productBase.Width;
            }
        }

        /// <summary>
        /// Gets the height.
        /// </summary>
        public decimal Height
        {
            get
            {
                return _productBase.Height;
            }
        }

        /// <summary>
        /// Gets the barcode.
        /// </summary>
        public string Barcode
        {
            get
            {
                return _productBase.Barcode;
            }
        }

        /// <summary>
        /// Gets a value indicating whether out of stock purchase.
        /// </summary>
        public bool OutOfStockPurchase
        {
            get
            {
                return _productBase.OutOfStockPurchase;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the product is a downloadable product.
        /// </summary>
        public bool Download
        {
            get
            {
                return _productBase.Download;
            }
        }

        /// <summary>
        /// Gets the downloadable file's Umbraco media id.
        /// </summary>
        public int DownloadMediaId
        {
            get
            {
                return _productBase.DownloadMediaId;
            }
        }


        /// <summary>
        /// Gets the total inventory count.
        /// </summary>
        public virtual int TotalInventoryCount
        {
            get
            {
                return _productBase.TotalInventoryCount;
            }
        }

        /// <summary>
        /// Gets the catalog inventories.
        /// </summary>
        public IEnumerable<CatalogInventoryDisplay> CatalogInventories
        {
            get
            {
                return _productBase.CatalogInventories;
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
        /// Gets the document type id.
        /// </summary>
        public override int DocumentTypeId
        {
            get
            {
                return _contentType != null ? _contentType.Id : 0;
            }
        }

        /// <summary>
        /// Gets the document type alias.
        /// </summary>
        public override string DocumentTypeAlias
        {
            get
            {
                return _contentType != null ? _contentType.Alias : null;
            }
        }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        public override ICollection<IPublishedProperty> Properties
        {
            get
            {
                return this._properties.Value[this.CultureName].ToArray();
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
                return 0;
            }
        }

        /// <summary>
        /// Gets the url name.
        /// </summary>
        public override string UrlName
        {
            get
            {
                return DetachedContentDisplay != null ? DetachedContentDisplay.Slug : null;
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
                return _productBase.VersionKey;
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
                return _detachedContentDisplay.CreateDate;
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
                return _detachedContentDisplay.UpdateDate;
            }
        }

        /// <summary>
        /// Gets the detached content display.
        /// </summary>
        protected ProductVariantDetachedContentDisplay DetachedContentDisplay
        {
            get
            {
                return _detachedContentDisplay;
            }
        }

        /// <summary>
        /// Gets the detached properties.
        /// </summary>
        protected IEnumerable<IPublishedProperty> DetachedProperties
        {
            get
            {
                return _properties.Value[this.CultureName];
            }
        }

        /// <summary>
        /// Gets the collection of <see cref="PublishedContentType"/> associated with product options.
        /// </summary>
        protected IDictionary<Guid, PublishedContentType> OptionContentTypes { get; }

        /// <summary>
        /// The get property.
        /// </summary>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <returns>
        /// The <see cref="IPublishedProperty"/>.
        /// </returns>
        public override IPublishedProperty GetProperty(string alias)
        {
            return _properties.Value[this.CultureName].FirstOrDefault(x => x.PropertyTypeAlias.InvariantEquals(alias));
        }

        /// <summary>
        /// The get property.
        /// </summary>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <param name="recurse">
        /// Indicates if this is a recursive property call.
        /// </param>
        /// <returns>
        /// The <see cref="IPublishedProperty"/>.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// Throws an exception on a recursive property get
        /// </exception>
        public override IPublishedProperty GetProperty(string alias, bool recurse)
        {
            if (recurse && Parent == null)
                throw new NotSupportedException("Parent must be set in order to recurse");

            var prop = GetProperty(alias);
            return prop == null && recurse ? 
                Parent.GetProperty(alias, true) 
                : prop;
        }

        /// <summary>
        /// Changes the current culture.
        /// </summary>
        /// <param name="cultureName">
        /// The culture name.
        /// </param>
        internal void ChangeCulture(string cultureName)
        {            
            CultureName = cultureName;
            _detachedContentDisplay = this.GetDetachedContentDisplayForCulture();
        }

        /// <summary>
        /// The build properties.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{IPublishedProperty}"/>.
        /// </returns>
        private Dictionary<string, IEnumerable<IPublishedProperty>> BuildProperties()
        {
            var propDictionary = new Dictionary<string, IEnumerable<IPublishedProperty>>();
            if (!_productBase.DetachedContents.Any() || _contentType == null) return propDictionary;

            foreach (var dc in _productBase.DetachedContents)
            {
                propDictionary.Add(dc.CultureName, dc.DataValuesAsPublishedProperties(this._contentType));
            }

            return propDictionary;
        }

        /// <summary>
        /// The get detached content display for culture.
        /// </summary>
        /// <returns>
        /// The <see cref="ProductVariantDetachedContentDisplay"/>.
        /// </returns>
        private ProductVariantDetachedContentDisplay GetDetachedContentDisplayForCulture()
        {
            return this._productBase.DetachedContents.Any(x => x.CultureName == this.CultureName) ?
                this._productBase.DetachedContents.FirstOrDefault(x => x.CultureName == this.CultureName) : 
                this._productBase.DetachedContents.FirstOrDefault(x => x.CultureName == Core.Constants.DefaultCultureName);
        }

        /// <summary>
        /// Initializes the model
        /// </summary>
        private void Initialize()
        {
            _detachedContentDisplay = this.GetDetachedContentDisplayForCulture();
            _properties = new Lazy<Dictionary<string, IEnumerable<IPublishedProperty>>>(this.BuildProperties);
        }
    }
}