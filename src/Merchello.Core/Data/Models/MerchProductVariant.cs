namespace Merchello.Core.Data.Models
{
    using System;
    using System.Collections.Generic;

    internal sealed partial class MerchProductVariant
    {
        public MerchProductVariant()
        {
            this.MerchCatalogInventory = new HashSet<CatalogInventoryDto>();
            this.MerchProductVariant2ProductAttribute = new HashSet<MerchProductVariant2ProductAttribute>();
            this.MerchProductVariantDetachedContent = new HashSet<MerchProductVariantDetachedContent>();
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

        public ICollection<MerchProductVariant2ProductAttribute> MerchProductVariant2ProductAttribute { get;
            set; }

        public ICollection<MerchProductVariantDetachedContent> MerchProductVariantDetachedContent { get; set; }

        public MerchProductVariantIndex MerchProductVariantIndex { get; set; }

        public MerchProduct ProductKeyNavigation { get; set; }
    }
}