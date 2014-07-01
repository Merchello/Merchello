namespace Merchello.Web.Models.ContentEditing
{
    using System.Collections.Generic;

    /// <summary>
    /// The product display abstract.
    /// </summary>
    public abstract class ProductDisplayBase
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the SKU.
        /// </summary>
        public string Sku { get; set; }

        /// <summary>
        /// Gets or sets the price.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the cost of goods.
        /// </summary>
        public decimal CostOfGoods { get; set; }

        /// <summary>
        /// Gets or sets the sale price.
        /// </summary>
        public decimal SalePrice { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether on sale.
        /// </summary>
        public bool OnSale { get; set; }

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
        /// Gets or sets a value indicating whether the product is available.
        /// </summary>
        public bool Available { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether track inventory.
        /// </summary>
        public bool TrackInventory { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether out of stock purchase.
        /// </summary>
        public bool OutOfStockPurchase { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether taxable.
        /// </summary>
        public bool Taxable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether shippable.
        /// </summary>
        public bool Shippable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product is a downloadable product.
        /// </summary>
        public bool Download { get; set; }

        /// <summary>
        /// Gets or sets the downloadable file's Umbraco media id.
        /// </summary>
        public int DownloadMediaId { get; set; }

        /// <summary>
        /// Gets or sets the catalog inventories.
        /// </summary>
        public IEnumerable<CatalogInventoryDisplay> CatalogInventories { get; set; }
    }
}