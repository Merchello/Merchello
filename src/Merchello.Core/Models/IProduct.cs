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
            /// The sku for the product
            /// </summary>
            [DataMember]
            string Sku { get; set;}
            
            /// <summary>
            /// The name of the product
            /// </summary>
            [DataMember]
            string Name { get; set;}
            
            /// <summary>
            /// The base price of the product
            /// </summary>
            [DataMember]
            decimal Price { get; set;}
            
            /// <summary>
            /// The cost of goods sold for the product
            /// </summary>
            [DataMember]
            decimal CostOfGoods { get; set;}
            
            /// <summary>
            /// The base "on sale" price for the product
            /// </summary>
            [DataMember]
            decimal SalePrice { get; set;}
            
            /// <summary>
            /// The brief description of the product
            /// </summary>
            [DataMember]
            string Brief { get; set;}
            
            /// <summary>
            /// The description of the product
            /// </summary>
            [DataMember]
            string Description { get; set;}
            
            /// <summary>
            /// Indicates whether or not to tax this product
            /// </summary>
            [DataMember]
            bool Taxable { get; set;}
            
            /// <summary>
            /// Indicates whether or not this product is shippable
            /// </summary>
            [DataMember]
            bool Shippable { get; set;}
            
            /// <summary>
            /// Indicates whether or not this product is avaialable via download
            /// </summary>
            [DataMember]
            bool Download { get; set;}
            
            /// <summary>
            /// The download url for the product
            /// </summary>
            [DataMember]
            string DownloadUrl { get; set;}
            
            /// <summary>
            /// Indicates whether or not this product is a template to be used when generating other products
            /// </summary>
            [DataMember]
            bool Template { get; set;}
    }
}



