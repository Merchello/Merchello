using System;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Tests.Base.Prototyping
{
    /// <summary>
    /// Defines a product item
    /// </summary>
    /// <remarks>
    /// Product items are essentially "Product Variants" and are what is actually used as basket, wish list, order, and invoice items.
    /// </remarks>
    public interface IProductItem : IKeyEntity
    {
        /// <summary>
        /// The product key for the item
        /// </summary>
        Guid ProductKey { get; }

        /// <summary>
        /// The sku unique sku of the item
        /// </summary>
        string Sku { get; set; }

        /// <summary>
        /// The price of the item
        /// </summary>
        decimal Price { get; set; }

        /// <summary>
        /// The shop's cost for the item
        /// </summary>
        decimal CostOfGoods { get; set; }

        /// <summary>
        /// The sale price of the item if on sale
        /// </summary>
        decimal SalePrice { get; set; }

        /// <summary>
        /// True/false indicating whether or not this item is onsale
        /// </summary>
        bool OnSale { get; set; }

        /// <summary>
        /// The weight of the item - intended to be used for shipping
        /// </summary>
        decimal Weight { get; set; }

        /// <summary>
        /// The length of the item - intended to be used for shipping
        /// </summary>
        decimal Length { get; set; }

        /// <summary>
        /// The width of the item  - intended to be used for shipping
        /// </summary>
        decimal Width { get; set; }

        /// <summary>
        /// The height of the item  - intended to be used for shipping
        /// </summary>
        decimal Height { get; set; }

        /// <summary>
        /// The optional barcode of the item
        /// </summary>
        string Barcode { get; set; }

        /// <summary>
        /// True/false indicating whether or not this product item is available for purchase.  This would override inventory if marked false.
        /// </summary>
        bool Available { get; set; }

        /// <summary>
        /// True/false indicating whether or not to track inventory on this product item
        /// </summary>
        bool TrackInventory { get; set; }

        /// <summary>
        /// True/false indicating whether or not this product item should be considered in tax computations
        /// </summary>
        bool Taxable { get; set; }

        /// <summary>
        /// True/false indicating whether or not this product item is able to be shipped, thus placed in a shipment
        /// </summary>
        bool Shippable { get; set; }

        /// <summary>
        /// True/false indicating whether or not this product is available for download
        /// </summary>
        bool Download { get; set; }

        /// <summary>
        /// The Umbraco MediaId of the download item
        /// </summary>
        bool DownloadMediaId { get; set; }

        /// <summary>
        /// True/false indicating whether or not this item is the template item use when creating other product items 
        /// for the associated product.
        /// </summary>
        /// <remarks>
        /// If the product is declared "singular", the template item is used as the product item
        /// </remarks>
        bool SingleItem { get; } 

        /// <summary>
        /// The product item's attributes
        /// </summary>
        /// <remarks>
        /// Designated options that make up this product item
        /// e.g. for product T-Shirt -> attributes could be  Small, Black
        /// </remarks>
        ProductAttributeCollection Attributes { get; }
    }
}