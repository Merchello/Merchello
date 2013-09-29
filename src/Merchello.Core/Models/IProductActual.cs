using System;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a Merchello Product object interface
    /// </summary>
    public interface IProductActual : IProductBase
    {
            
        /// <summary>
        /// The costOfGoods for the Product
        /// </summary>
        [DataMember]
        decimal? CostOfGoods { get; set;}
            
        /// <summary>
        /// The salePrice for the Product
        /// </summary>
        [DataMember]
        decimal? SalePrice { get; set;}
            
        /// <summary>
        /// The weight for the Product
        /// </summary>
        [DataMember]
        decimal? Weight { get; set;}
            
        /// <summary>
        /// The length for the Product
        /// </summary>
        [DataMember]
        decimal? Length { get; set;}
            
        /// <summary>
        /// The width for the Product
        /// </summary>
        [DataMember]
        decimal? Width { get; set;}
            
        /// <summary>
        /// The height for the Product
        /// </summary>
        [DataMember]
        decimal? Height { get; set;}

        /// <summary>
        /// The barcode of the product
        /// </summary>
        [DataMember]
        string Barcode { get; set; }

        /// <summary>
        /// True/false indicating whether or not this product is available
        /// </summary>
        [DataMember]
        bool Available { get; set; }

        /// <summary>
        /// True/false indicating whether or not to track inventory on this product
        /// </summary>
        [DataMember]
        bool TrackInventory { get; set; }         
          
        /// <summary>
        /// The taxable for the Product
        /// </summary>
        [DataMember]
        bool Taxable { get; set;}
            
        /// <summary>
        /// The shippable for the Product
        /// </summary>
        [DataMember]
        bool Shippable { get; set;}
            
        /// <summary>
        /// The download for the Product
        /// </summary>
        [DataMember]
        bool Download { get; set;}
            
        /// <summary>
        /// The downloadUrl for the Product
        /// </summary>
        [DataMember]
        string DownloadUrl { get; set;}
            
        /// <summary>
        /// The template for the Product
        /// </summary>
        [DataMember]
        bool Template { get; set;}
    }
}



