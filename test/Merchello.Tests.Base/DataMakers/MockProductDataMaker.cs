using System;
using System.Collections;
using System.Collections.Generic;
using Merchello.Core.Models;

namespace Merchello.Tests.Base.DataMakers
{
    /// <summary>
    /// Helper class to assist in putting together product data for testing
    /// </summary>
    public class MockProductDataMaker : MockDataMakerBase
    {

        public static IProductActual MockProductForInserting()
        {
            return MockProductForInserting(MockSku(), ProductItemName(), PriceCheck());
        }

        /// <summary>
        /// Represents a product as if it was returned from the database
        /// </summary>
        /// <param name="key">The key you want to use as the key for the product</param>
        /// <returns><see cref="IProductActual"/></returns>
        public static IProductActual MockProductComplete(Guid key)
        {
            var product = MockProductForInserting();
            product.Key = key;
            product.ResetDirtyProperties();
            return product;
        }

        /// <summary>
        /// Makes a list of products for inserting
        /// </summary>
        /// <param name="count">The number of products to create</param>
        /// <returns>A collection of <see cref="IProductActual"/></returns>
        public static IEnumerable<IProductActual> MockProductCollectionForInserting(int count)
        {
            for (var i = 0; i < count; i++) yield return MockProductForInserting(Guid.NewGuid().ToString().Replace("-", string.Empty), ProductItemName(), PriceCheck());
        }

        public static IProductActual MockProductForInserting(string sku, string name, decimal price)
        {
            return new ProductActual()
            {
                Sku = sku,
                Name = name,
                Price = price,
                CostOfGoods = null,
                SalePrice = null,
                Weight = null,
                Length = null,
                Width = null,
                Height = null,
                Taxable = true,
                Shippable = false,
                Download = false,
                Template = false
            };
        }

      

        
    }
}
