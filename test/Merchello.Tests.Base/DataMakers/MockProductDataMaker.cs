﻿using System;
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

        public static IProduct MockProductForInserting()
        {
            return MockProductForInserting(ProductItemName(), MockSku(), PriceCheck());
        }

        /// <summary>
        /// Represents a product as if it was returned from the database
        /// </summary>
        /// <param name="key">The key you want to use as the key for the product</param>
        /// <returns><see cref="IProductVariant"/></returns>
        public static IProduct MockProductComplete(Guid key)
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
        /// <returns>A collection of <see cref="IProductVariant"/></returns>
        public static IEnumerable<IProduct> MockProductCollectionForInserting(int count)
        {
            for (var i = 0; i < count; i++) yield return MockProductForInserting(ProductItemName(), Guid.NewGuid().ToString().Replace("-", string.Empty), PriceCheck());
        }

        public static IProduct MockProductForInserting(string name, string sku, decimal price)
        {
            return new Product(new ProductVariant(name, sku, price));
        }
    }
}
