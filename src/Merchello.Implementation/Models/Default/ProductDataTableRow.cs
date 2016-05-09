namespace Merchello.Implementation.Models
{
    using System;

    /// <summary>
    /// An object intended to be used to pass serialized data to JS objects.
    /// </summary>
    public class ProductDataTableRow : IProductDataTableRow
    {
        /// <summary>
        /// Gets or sets the product key.
        /// </summary>
        public Guid ProductKey { get; set; }

        /// <summary>
        /// Gets or sets the product variant key.
        /// </summary>
        public Guid ProductVariantKey { get; set; }

        /// <summary>
        /// Gets or sets the formatted sale price.
        /// </summary>
        public string FormattedSalePrice { get; set; }

        /// <summary>
        /// Gets or sets the SKU.
        /// </summary>
        public string Sku { get; set; }

        /// <summary>
        /// Gets or sets the match keys.
        /// </summary>
        public Guid[] MatchKeys { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether on sale.
        /// </summary>
        public bool OnSale { get; set; }

        /// <summary>
        /// Gets or sets the price.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the formatted price.
        /// </summary>
        public string FormattedPrice { get; set; }

        /// <summary>
        /// Gets or sets the sale price.
        /// </summary>
        public decimal SalePrice { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is available.
        /// </summary>
        public bool IsAvailable { get; set; }

        /// <summary>
        /// Gets or sets the inventory count.
        /// </summary>
        public int InventoryCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether out of stock purchase is allowed.
        /// </summary>
        public bool OutOfStockPurchase { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is the pricing is for a product variant.
        /// </summary>
        public bool IsForVariant { get; set; }
    }
}