namespace Merchello.Web.Models.VirtualContent
{
    /// <summary>
    /// The ProductContentBase interface.
    /// </summary>
    public interface IProductContentBase
    {
        /// <summary>
        /// Gets the culture name.
        /// </summary>
        string CultureName { get; }

        /// <summary>
        /// Gets the price.
        /// </summary>
        decimal Price { get; }

        /// <summary>
        /// Gets the sale price.
        /// </summary>
        decimal SalePrice { get; }

        /// <summary>
        /// Gets a value indicating whether on sale.
        /// </summary>
        bool OnSale { get; }

        /// <summary>
        /// Gets a value indicating whether available.
        /// </summary>
        bool Available { get; }

        /// <summary>
        /// Gets a value indicating whether track inventory.
        /// </summary>
        bool TrackInventory { get; }

        /// <summary>
        /// Gets a value indicating whether shippable.
        /// </summary>
        bool Shippable { get; }

        /// <summary>
        /// Gets a value indicating whether taxable.
        /// </summary>
        bool Taxable { get; }

        /// <summary>
        /// Gets the SKU.
        /// </summary>
        string Sku { get; }

        /// <summary>
        /// Gets the cost of goods.
        /// </summary>
        decimal CostOfGoods { get; }

        /// <summary>
        /// Gets the manufacturer.
        /// </summary>
        string Manufacturer { get; }

        /// <summary>
        /// Gets the manufacturer model number.
        /// </summary>
        string ManufacturerModelNumber { get; }

        /// <summary>
        /// Gets the weight.
        /// </summary>
        decimal Weight { get; }

        /// <summary>
        /// Gets the length.
        /// </summary>
        decimal Length { get; }

        /// <summary>
        /// Gets the width.
        /// </summary>
        decimal Width { get; }

        /// <summary>
        /// Gets the height.
        /// </summary>
        decimal Height { get; }

        /// <summary>
        /// Gets the barcode.
        /// </summary>
        string Barcode { get; }

        /// <summary>
        /// Gets a value indicating whether out of stock purchase.
        /// </summary>
        bool OutOfStockPurchase { get; }

        /// <summary>
        /// Gets a value indicating whether the product is a downloadable product.
        /// </summary>
        bool Download { get; }

        /// <summary>
        /// Gets the downloadable file's Umbraco media id.
        /// </summary>
        int DownloadMediaId { get; }

        /// <summary>
        /// Gets the total inventory count.
        /// </summary>
        int TotalInventoryCount { get; }
    }
}