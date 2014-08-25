using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchProductVariant")]
    [PrimaryKey("pk", autoIncrement = false)]
    [ExplicitColumns]
    public class ProductVariantDto : IPageableDto
    {
        [Column("pk")]
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Constraint(Default = "newid()")]
        public Guid Key { get; set; }

        [Column("productKey")]
        [ForeignKey(typeof(ProductDto), Name = "FK_merchProductVariant_merchProduct", Column = "pk")]
        public Guid ProductKey { get; set; }

        [Column("sku")]
        public string Sku { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("price")]
        public decimal Price { get; set; }

        [Column("costOfGoods")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public decimal? CostOfGoods { get; set; }

        [Column("salePrice")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public decimal? SalePrice { get; set; }

        [Column("onSale")]
        public bool OnSale { get; set; }

        [Column("manufacturer")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string Manufacturer { get; set; }

        [Column("modelNumber")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string ManufacturerModelNumber { get; set; } 

        [Column("weight")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public decimal? Weight { get; set; }

        [Column("length")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public decimal? Length { get; set; }

        [Column("width")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public decimal? Width { get; set; }

        [Column("height")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public decimal? Height { get; set; }

        [Column("barcode")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string Barcode { get; set; }

        [Column("available")]
        public bool Available { get; set; }

        [Column("trackInventory")]
        [Constraint(Default = "1")]
        public bool TrackInventory { get; set; }

        [Column("outOfStockPurchase")]
        public bool OutOfStockPurchase { get; set; }

        [Column("taxable")]
        [Constraint(Default = "1")]
        public bool Taxable { get; set; }

        [Column("shippable")]
        [Constraint(Default = "1")]
        public bool Shippable { get; set; }

        [Column("download")]
        [Constraint(Default = "0")]
        public bool Download { get; set; }

        [Column("downloadMediaId")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public int? DownloadMediaId { get; set; }

        [Column("master")]
        [Constraint(Default = "0")]
        public bool Master { get; set; }

        [Column("versionKey")]
        [Constraint(Default = "newid()")]
        public Guid VersionKey { get; set; }

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }

        [ResultColumn]
        public ProductVariantIndexDto ProductVariantIndexDto { get; set; }
    }
}
