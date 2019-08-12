namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.DetachedContent;

    /// <summary>
    /// Defines a product
    /// </summary>
    public interface IProductBase
    {
        /// <summary>
        /// Gets or sets the SKU for the Product
        /// </summary>
        [DataMember]
        string Sku { get; set; }

        /// <summary>
        /// Gets or sets the name for the Product
        /// </summary>
        [DataMember]
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the price for the Product
        /// </summary>
        [DataMember]
        decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the shop's cost for the product
        /// </summary>
        [DataMember]
        decimal? CostOfGoods { get; set; }

        /// <summary>
        /// Gets or sets the sale price of the product if on sale
        /// </summary>
        [DataMember]
        decimal? SalePrice { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not this product is on sale
        /// </summary>
        [DataMember]
        bool OnSale { get; set; }

        /// <summary>
        /// Gets or sets the manufacturer of the product
        /// </summary>
        [DataMember]
        string Manufacturer { get; set; }

        /// <summary>
        /// Gets or sets the manufacturer model number of the product
        /// </summary>
        [DataMember]
        string ManufacturerModelNumber { get; set; }

        /// <summary>
        /// Gets or sets the weight of the product - intended to be used for shipping
        /// </summary>
        [DataMember]
        decimal? Weight { get; set; }

        /// <summary>
        /// Gets or sets the length of the product - intended to be used for shipping
        /// </summary>
        [DataMember]
        decimal? Length { get; set; }

        /// <summary>
        /// Gets or sets the width of the product  - intended to be used for shipping
        /// </summary>
        [DataMember]
        decimal? Width { get; set; }

        /// <summary>
        /// Gets or sets the height of the product  - intended to be used for shipping
        /// </summary>
        decimal? Height { get; set; }

        /// <summary>
        /// Gets or sets the optional barcode of the product
        /// </summary>
        [DataMember]
        string Barcode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not this product is available for purchase.  This would override inventory if marked false.
        /// </summary>
        [DataMember]
        bool Available { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not to track inventory on this product
        /// </summary>
        [DataMember]
        bool TrackInventory { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not this product can be purchased when inventory levels are 
        /// 0 or below.
        /// </summary>
        [DataMember]
        bool OutOfStockPurchase { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not this product should be considered in tax computations
        /// </summary>
        [DataMember]
        bool Taxable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not this product is able to be shipped, thus placed in a shipment
        /// </summary>
        [DataMember]
        bool Shippable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not this product is available for download
        /// </summary>
        [DataMember]
        bool Download { get; set; }

        /// <summary>
        /// Gets or sets the Umbraco MediaId of the download product
        /// </summary>
        [DataMember]
        int? DownloadMediaId { get; set; }

        /// <summary>
        /// Gets the version key.
        /// </summary>
        [DataMember]
        Guid VersionKey { get;  }

        /// <summary>
        /// Gets product inventory
        /// </summary>
        IEnumerable<ICatalogInventory> CatalogInventories { get; }

        /// <summary>
        /// Gets the detached contents.
        /// </summary>
        DetachedContentCollection<IProductVariantDetachedContent> DetachedContents { get; }
    }

}