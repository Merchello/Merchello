using System;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a Merchello Product object interface
    /// </summary>
    public interface IProduct : IKeyEntity
    {
            
            
            /// <summary>
            /// The sku for the Product
            /// </summary>
            [DataMember]
            string Sku { get; set;}
            
            /// <summary>
            /// The name for the Product
            /// </summary>
            [DataMember]
            string Name { get; set;}
            
            /// <summary>
            /// The price for the Product
            /// </summary>
            [DataMember]
            decimal Price { get; set;}
            
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



