namespace Merchello.Core.Models.Rdbms
{
    using System;

    using JetBrains.Annotations;

    /// <summary>
    /// The table definition and "POCO" for database operations associated with the "merchProductVariant" table.
    /// </summary>
    internal class ProductVariantDto : IEntityDto, IPageableDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the product key.
        /// </summary>
        public Guid ProductKey { get; set; }

        /// <summary>
        /// Gets or sets the SKU.
        /// </summary>
        public string Sku { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the price.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the cost of goods.
        /// </summary>
        [CanBeNull]
        public decimal? CostOfGoods { get; set; }

        /// <summary>
        /// Gets or sets the sale price.
        /// </summary>
        [CanBeNull]
        public decimal? SalePrice { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether on sale.
        /// </summary>
        public bool OnSale { get; set; }

        /// <summary>
        /// Gets or sets the manufacturer.
        /// </summary>
        [CanBeNull]
        public string Manufacturer { get; set; }

        /// <summary>
        /// Gets or sets the manufacturer model number.
        /// </summary>
        [CanBeNull]
        public string ManufacturerModelNumber { get; set; }

        /// <summary>
        /// Gets or sets the weight.
        /// </summary>
        [CanBeNull]
        public decimal? Weight { get; set; }

        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        [CanBeNull]
        public decimal? Length { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        [CanBeNull]
        public decimal? Width { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        [CanBeNull]
        public decimal? Height { get; set; }

        /// <summary>
        /// Gets or sets the barcode.
        /// </summary>
        [CanBeNull]
        public string Barcode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether available.
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
        /// Gets or sets a value indicating whether download.
        /// </summary>
        public bool Download { get; set; }

        /// <summary>
        /// Gets or sets the download media id.
        /// </summary>
        [CanBeNull]
        public int? DownloadMediaId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this variant is the master variant.
        /// </summary>
        public bool Master { get; set; }


        /// <summary>
        /// Gets or sets the version key.
        /// </summary>
        public Guid VersionKey { get; set; }

        /// <inheritdoc/>
        public DateTime UpdateDate { get; set; }

        /// <inheritdoc/>
        public DateTime CreateDate { get; set; }
    }
}
