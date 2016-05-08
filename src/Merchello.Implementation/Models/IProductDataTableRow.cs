namespace Merchello.Implementation.Models
{
    using System;

    /// <summary>
    /// Defines a product data table row.
    /// </summary>
    /// <remarks>
    /// Used to preload JavaScript pricing table to reduce the number of roundtrip AJAX transactions
    /// when selecting product variants to add to the basket
    /// </remarks>
    public interface IProductDataTableRow
    {
        /// <summary>
        /// Gets or sets the product key.
        /// </summary>
        Guid ProductKey { get; set; }

        /// <summary>
        /// Gets or sets the product variant key.
        /// </summary>
        Guid ProductVariantKey { get; set; }

        /// <summary>
        /// Gets or sets the price.
        /// </summary>
        decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the sale price.
        /// </summary>
        decimal SalePrice { get; set; }

        /// <summary>
        /// Gets or sets the SKU.
        /// </summary>
        string Sku { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether on sale.
        /// </summary>
        bool OnSale { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether out of stock purchase.
        /// </summary>
        bool OutOfStockPurchase { get; set; }

        /// <summary>
        /// Gets or sets the inventory count.
        /// </summary>
        int InventoryCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is available.
        /// </summary>
        bool IsAvailable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is for variant.
        /// </summary>
        bool IsForVariant { get; set; }

        /// <summary>
        /// Gets or sets the match keys.
        /// </summary>
        Guid[] MatchKeys { get; set; }
    }
}