using System.Collections.Generic;
using Merchello.Tests.Base.Prototyping.Models;

namespace Merchello.Core.Models
{
    public interface IProduct : IProductBase
    {
        /// <summary>
        /// True/false indicating whether or not this product group defines product options.
        /// e.g. The product has no required options
        /// </summary>
         bool DefinesOptions { get; }

         /// <summary>
         /// The product's collection of options (Attribute selection)
         /// </summary>
         ProductOptionCollection ProductOptions { get; }
    }
}