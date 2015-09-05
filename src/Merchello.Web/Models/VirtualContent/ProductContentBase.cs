namespace Merchello.Web.Models.VirtualContent
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Web.Models.ContentEditing;
    using Merchello.Web.Models.ContentEditing.Content;
    using Merchello.Web.Models.DetachedContent;

    using Umbraco.Core;
    using Umbraco.Core.Models;
    using Umbraco.Web.Models;

    /// <summary>
    /// Base class for Product content classes
    /// </summary>
    internal abstract class ProductContentBase : PublishedContentBase
    {
        /// <summary>
        /// The product base.
        /// </summary>
        private readonly ProductDisplayBase _productBase;

        /// <summary>
        /// The culture name.
        /// </summary>
        private readonly string _cultureName;

        /// <summary>
        /// The properties.
        /// </summary>
        private Lazy<IEnumerable<IPublishedProperty>> _properties;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductContentBase"/> class.
        /// </summary>
        /// <param name="productBase">
        /// The product base.
        /// </param>
        /// <param name="cultureName">
        /// The culture name
        /// </param>
        protected ProductContentBase(ProductDisplayBase productBase, string cultureName = "en-US")
        {
            Mandate.ParameterNotNull(productBase, "productBase");
            _productBase = productBase;
            _cultureName = cultureName;
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
        /// Gets the properties.
        /// </summary>
        public override ICollection<IPublishedProperty> Properties
        {
            get
            {
                return this._properties.Value.ToArray();
            }
        }

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
            return _properties.Value.FirstOrDefault(x => x.PropertyTypeAlias.InvariantEquals(alias));
        }

        /// <summary>
        /// The get property.
        /// </summary>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <param name="recurse">
        /// The recurse.
        /// </param>
        /// <returns>
        /// The <see cref="IPublishedProperty"/>.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public override IPublishedProperty GetProperty(string alias, bool recurse)
        {
            if (recurse)
                throw new NotSupportedException();

            return GetProperty(alias);
        }

        /// <summary>
        /// Gets the detached properties.
        /// </summary>
        protected IEnumerable<IPublishedProperty> DetachedProperties
        {
            get
            {
                return _properties.Value;
            }
        }

        private void Initialize()
        {
            
        }
    }
}