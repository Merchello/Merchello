using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Tests.Base.Prototyping.Models
{
    /// <summary>
    /// Defines a product
    /// </summary>
    public interface IProductBase : IKeyEntity
    {
        /// <summary>
        /// The sku for the Product
        /// </summary>
        [DataMember]
        string Sku { get; set; }

        /// <summary>
        /// The name for the Product
        /// </summary>
        [DataMember]
        string Name { get; set; }

        /// <summary>
        /// The price for the Product
        /// </summary>
        [DataMember]
        decimal Price { get; set; }

        /// <summary>
        /// The shop's cost for the product
        /// </summary>
        [DataMember]
        decimal? CostOfGoods { get; set; }

        /// <summary>
        /// The sale price of the product if on sale
        /// </summary>
        [DataMember]
        decimal? SalePrice { get; set; }

        /// <summary>
        /// True/false indicating whether or not this product is onsale
        /// </summary>
        [DataMember]
        bool OnSale { get; set; }

        /// <summary>
        /// The weight of the product - intended to be used for shipping
        /// </summary>
        [DataMember]
        decimal? Weight { get; set; }

        /// <summary>
        /// The length of the product - intended to be used for shipping
        /// </summary>
        [DataMember]
        decimal? Length { get; set; }

        /// <summary>
        /// The width of the product  - intended to be used for shipping
        /// </summary>
        [DataMember]
        decimal? Width { get; set; }

        /// <summary>
        /// The height of the product  - intended to be used for shipping
        /// </summary>
        decimal? Height { get; set; }

        /// <summary>
        /// The optional barcode of the product
        /// </summary>
        [DataMember]
        string Barcode { get; set; }

        /// <summary>
        /// True/false indicating whether or not this product is available for purchase.  This would override inventory if marked false.
        /// </summary>
        [DataMember]
        bool Available { get; set; }

        /// <summary>
        /// True/false indicating whether or not to track inventory on this product
        /// </summary>
        [DataMember]
        bool TrackInventory { get; set; }

        /// <summary>
        /// True/false indicating wether or not this product can be purchased when inventory levels are 
        /// 0 or below.
        /// </summary>
        [DataMember]
        bool OutOfStockPurchase { get; set; }

        /// <summary>
        /// True/false indicating whether or not this product should be considered in tax computations
        /// </summary>
        [DataMember]
        bool Taxable { get; set; }

        /// <summary>
        /// True/false indicating whether or not this product is able to be shipped, thus placed in a shipment
        /// </summary>
        [DataMember]
        bool Shippable { get; set; }

        /// <summary>
        /// True/false indicating whether or not this product is available for download
        /// </summary>
        [DataMember]
        bool Download { get; set; }

        /// <summary>
        /// The Umbraco MediaId of the download product
        /// </summary>
        [DataMember]
        int? DownloadMediaId { get; set; }

        /// <summary>
        /// The product inventory
        /// </summary>
        InventoryCollection Inventory { get; }
    }

}