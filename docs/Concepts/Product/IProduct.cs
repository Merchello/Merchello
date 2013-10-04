using System.ComponentModel;
using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;
using Umbraco.Core;

namespace Merchello.Tests.Base.Prototyping
{
    /// <summary>
    /// Defines a product
    /// </summary>
    public interface IProduct : IProductItem
    {
        /// <summary>
        /// True/false indicating whether or not this product can be considered as a singular product.
        /// e.g. The product has not required options
        /// </summary>
        bool Singular { get; set; }

        /// <summary>
        /// True/false indicating wether or not this product can be purchased when inventory levels are 
        /// 0 or below.
        /// </summary>
        bool OutOfStockPurchase { get; set; }

        /// <summary>
        /// The product's product item collection
        /// </summary>
        ProductItemCollection ProductItems { get; }

        /// <summary>
        /// The product's collection of options (Attribute selection)
        /// </summary>
        OptionCollection Options { get; }

    }
}