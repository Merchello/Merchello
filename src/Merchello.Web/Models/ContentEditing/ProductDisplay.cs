namespace Merchello.Web.Models.ContentEditing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The product display.
    /// </summary>
    public class ProductDisplay : ProductDisplayBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductDisplay"/> class.
        /// </summary>
        public ProductDisplay()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductDisplay"/> class.
        /// </summary>
        /// <param name="masterVariant">
        /// The master variant.
        /// </param>
        internal ProductDisplay(ProductVariantDisplay masterVariant)
        {
            Key = masterVariant.ProductKey;
            ProductVariantKey = masterVariant.Key;
            VersionKey = masterVariant.VersionKey;
            Name = masterVariant.Name;
            Sku = masterVariant.Sku;
            Price = masterVariant.Price;
            CostOfGoods = masterVariant.CostOfGoods;
            SalePrice = masterVariant.SalePrice;
            OnSale = masterVariant.OnSale;
            Manufacturer = masterVariant.Manufacturer;
            ManufacturerModelNumber = masterVariant.ManufacturerModelNumber;
            Weight = masterVariant.Weight;
            Length = masterVariant.Length;
            Height = masterVariant.Height;
            Width = masterVariant.Width;
            Barcode = masterVariant.Barcode;
            Available = masterVariant.Available;
            TrackInventory = masterVariant.TrackInventory;
            OutOfStockPurchase = masterVariant.OutOfStockPurchase;
            Taxable = masterVariant.Taxable;
            Shippable = masterVariant.Shippable;
            Download = masterVariant.Download;
            DownloadMediaId = masterVariant.DownloadMediaId;
            CatalogInventories = masterVariant.CatalogInventories;
            DetachedContents = masterVariant.DetachedContents;
            VersionKey = masterVariant.VersionKey;
        }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the product variant key of the master variant.
        /// </summary>
        public Guid ProductVariantKey { get; set; }

        /// <summary>
        /// Gets or sets the virtual variants property
        /// </summary>
        public bool VirtualVariants{ get; set; }

        /// <summary>
        /// Gets or sets the product options.
        /// </summary>
        public IEnumerable<ProductOptionDisplay> ProductOptions { get; set; }

        /// <summary>
        /// Gets or sets the product variants.
        /// </summary>
        public IEnumerable<ProductVariantDisplay> ProductVariants { get; set; }

        /// <summary>
        /// Gets the total inventory count.
        /// </summary>
        public override int TotalInventoryCount
        {
            get
            {
                return this.ProductVariants != null
                           ? this.ProductVariants.Any()
                                 ? this.ProductVariants.Sum(x => x.TotalInventoryCount)
                                 : this.CatalogInventories.Sum(x => x.Count)
                           : 0;
            }
        }
    }
}
