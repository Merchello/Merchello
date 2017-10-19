namespace Merchello.Core.Data.Models
{
    using System;
    using System.Collections.Generic;

    internal sealed class ProductVariantDto
    {
        public ProductVariantDto()
        {
            this.MerchCatalogInventory = new HashSet<CatalogInventoryDto>();
            this.MerchProductVariant2ProductAttribute = new HashSet<ProductVariant2ProductAttributeDto>();
            this.MerchProductVariantDetachedContent = new HashSet<ProductVariantDetachedContentDto>();
        }

        public Guid Pk { get; set; }

        public Guid ProductKey { get; set; }

        public string Sku { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public decimal? CostOfGoods { get; set; }

        public decimal? SalePrice { get; set; }

        public bool OnSale { get; set; }

        public string Manufacturer { get; set; }

        public string ModelNumber { get; set; }

        public decimal? Weight { get; set; }

        public decimal? Length { get; set; }

        public decimal? Width { get; set; }

        public decimal? Height { get; set; }

        public string Barcode { get; set; }

        public bool Available { get; set; }

        public bool TrackInventory { get; set; }

        public bool OutOfStockPurchase { get; set; }

        public bool Taxable { get; set; }

        public bool Shippable { get; set; }

        public bool Download { get; set; }

        public int? DownloadMediaId { get; set; }

        public bool Master { get; set; }

        public Guid VersionKey { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public ICollection<CatalogInventoryDto> MerchCatalogInventory { get; set; }

        public ICollection<ProductVariant2ProductAttributeDto> MerchProductVariant2ProductAttribute { get;
            set; }

        public ICollection<ProductVariantDetachedContentDto> MerchProductVariantDetachedContent { get; set; }

        public ProductVariantIndexDto ProductVariantIndexDto { get; set; }

        public ProductDto ProductDtoKeyNavigation { get; set; }
    }
}