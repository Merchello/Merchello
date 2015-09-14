namespace Merchello.Web.Models.ContentEditing
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Models;
    using Merchello.Web.Models.ContentEditing.Content;

    /// <summary>
    /// The product display abstract.
    /// </summary>
    public abstract class ProductDisplayBase : ProductVariantDataModifierData
    {
        /// <summary>
        /// Gets or sets the version key.
        /// </summary>
        public Guid VersionKey { get; set; }

        /// <summary>
        /// Gets or sets the SKU.
        /// </summary>
        public string Sku { get; set; }

        /// <summary>
        /// Gets or sets the cost of goods.
        /// </summary>
        public decimal CostOfGoods { get; set; }

        /// <summary>
        /// Gets or sets the manufacturer.
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// Gets or sets the manufacturer model number.
        /// </summary>
        public string ManufacturerModelNumber { get; set; }

        /// <summary>
        /// Gets or sets the weight.
        /// </summary>
        public decimal Weight { get; set; }

        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        public decimal Length { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        public decimal Width { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        public decimal Height { get; set; }

        /// <summary>
        /// Gets or sets the barcode.
        /// </summary>
        public string Barcode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether out of stock purchase.
        /// </summary>
        public bool OutOfStockPurchase { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product is a downloadable product.
        /// </summary>
        public bool Download { get; set; }

        /// <summary>
        /// Gets or sets the downloadable file's Umbraco media id.
        /// </summary>
        public int DownloadMediaId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether master.
        /// </summary>
        public bool Master { get; set; }

        /// <summary>
        /// Gets or sets the total inventory count.
        /// </summary>
        public virtual int TotalInventoryCount { get; set; }

        /// <summary>
        /// Gets or sets the catalog inventories.
        /// </summary>
        public IEnumerable<CatalogInventoryDisplay> CatalogInventories { get; set; }

        /// <summary>
        /// Gets or sets the detached content values.
        /// </summary>
        public IEnumerable<ProductVariantDetachedContentDisplay> DetachedContents { get; set; } 
    }
}