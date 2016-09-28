namespace Merchello.Core.Models.Rdbms
{
    using System;

    using Merchello.Core.Acquired.Persistence.DatabaseAnnotations;

    using NPoco;

    /// <summary>
    /// The table definition and "POCO" for database operations associated with the "merchProductVariant" table.
    /// </summary>
    [TableName("merchProductVariant")]
    [PrimaryKey("pk", AutoIncrement = false)]
    [ExplicitColumns]
    internal class ProductVariantDto : EntityDto, IPageableDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Column("pk")]
        [Constraint(Default = "newid()")]
        public override Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the product key.
        /// </summary>
        [Column("productKey")]
        [ForeignKey(typeof(ProductDto), Name = "FK_merchProductVariant_merchProduct", Column = "pk")]
        public Guid ProductKey { get; set; }

        /// TODO add index to SKU http://issues.merchello.com/youtrack/issue/M-646
        /// <summary>
        /// Gets or sets the SKU.
        /// </summary>
        [Column("sku")]
        [Index(IndexTypes.UniqueNonClustered, Name = "IX_merchProductVariantSku")]
        public string Sku { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [Column("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the price.
        /// </summary>
        [Column("price")]
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the cost of goods.
        /// </summary>
        [Column("costOfGoods")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public decimal? CostOfGoods { get; set; }

        /// <summary>
        /// Gets or sets the sale price.
        /// </summary>
        [Column("salePrice")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public decimal? SalePrice { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether on sale.
        /// </summary>
        [Column("onSale")]
        public bool OnSale { get; set; }

        /// <summary>
        /// Gets or sets the manufacturer.
        /// </summary>
        [Column("manufacturer")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string Manufacturer { get; set; }

        /// <summary>
        /// Gets or sets the manufacturer model number.
        /// </summary>
        [Column("modelNumber")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string ManufacturerModelNumber { get; set; }

        /// <summary>
        /// Gets or sets the weight.
        /// </summary>
        [Column("weight")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public decimal? Weight { get; set; }

        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        [Column("length")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public decimal? Length { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        [Column("width")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public decimal? Width { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        [Column("height")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public decimal? Height { get; set; }

        /// <summary>
        /// Gets or sets the barcode.
        /// </summary>
        [Column("barcode")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string Barcode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether available.
        /// </summary>
        [Column("available")]
        public bool Available { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether track inventory.
        /// </summary>
        [Column("trackInventory")]
        [Constraint(Default = "1")]
        public bool TrackInventory { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether out of stock purchase.
        /// </summary>
        [Column("outOfStockPurchase")]
        public bool OutOfStockPurchase { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether taxable.
        /// </summary>
        [Column("taxable")]
        [Constraint(Default = "1")]
        public bool Taxable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether shippable.
        /// </summary>
        [Column("shippable")]
        [Constraint(Default = "1")]
        public bool Shippable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether download.
        /// </summary>
        [Column("download")]
        [Constraint(Default = "0")]
        public bool Download { get; set; }

        /// <summary>
        /// Gets or sets the download media id.
        /// </summary>
        [Column("downloadMediaId")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public int? DownloadMediaId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this variant is the master variant.
        /// </summary>
        [Column("master")]
        [Constraint(Default = "0")]
        public bool Master { get; set; }


        /// <summary>
        /// Gets or sets the version key.
        /// </summary>
        [Column("versionKey")]
        [Constraint(Default = "newid()")]
        public Guid VersionKey { get; set; }

        /// <summary>
        /// Gets or sets the product variant index dto.
        /// </summary>
        [ResultColumn]
        public ProductVariantIndexDto ProductVariantIndexDto { get; set; }

    }
}
