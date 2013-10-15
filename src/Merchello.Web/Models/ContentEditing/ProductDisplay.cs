using System;
using System.Collections.Generic;
using Merchello.Core.Models;

namespace Merchello.Web.Models.ContentEditing
{
    public class ProductDisplay
    {
        public ProductDisplay()
        {
        }

        internal ProductDisplay(Product fromProduct)
        {
            Key = fromProduct.Key;
            Name = fromProduct.Name;
            Sku = fromProduct.Sku;
            Price = fromProduct.Price;
        }

        public Guid Key { get; set; }
        public string Name { get; set; }
        public string Sku { get; set; }
        public decimal Price { get; set; }

    }
}
