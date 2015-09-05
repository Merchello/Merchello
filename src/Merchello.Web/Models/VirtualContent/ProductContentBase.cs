namespace Merchello.Web.Models.VirtualContent
{
    using System;
    using System.Collections.Generic;

    using Merchello.Web.Models.ContentEditing;

    using Umbraco.Core;

    /// <summary>
    /// Base class for Product content classes
    /// </summary>
    public abstract class ProductContentBase 
    {
        /// <summary>
        /// The product base.
        /// </summary>
        private readonly ProductDisplayBase _productBase;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductContentBase"/> class.
        /// </summary>
        /// <param name="productBase">
        /// The product base.
        /// </param>
        protected ProductContentBase(ProductDisplayBase productBase)
        {
            Mandate.ParameterNotNull(productBase, "productBase");
            _productBase = productBase;
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
    }
}