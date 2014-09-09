using System;
using System.Collections;
using System.Collections.Generic;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Web.Models.ContentEditing;
using NUnit.Framework;
using umbraco.presentation.umbraco.dialogs;

namespace Merchello.Tests.Base.DataMakers
{
    /// <summary>
    /// Helper class to assist in putting together product data for testing
    /// </summary>
    public class MockProductDataMaker : MockDataMakerBase
    {

        public static IProduct MockProductForInserting(bool shippable = true, decimal weight = 0, decimal price = 0, bool taxable = true)
        {
            var mockPrice = price == 0 ? PriceCheck() : price;
            var product = MockProductForInserting(ProductItemName(), MockSku(), mockPrice, weight, taxable);
            product.Shippable = shippable;
            return product;
        }

        /// <summary>
        /// Represents a product as if it was returned from the database
        /// </summary>
        /// <param name="key">The key you want to use as the key for the product</param>
        /// <returns><see cref="IProduct"/></returns>
        public static IProduct MockProductComplete(Guid key)
        {
            var product = MockProductForInserting();            
            ((Product)product).AddingEntity();
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

        public static IProduct MockProductForInserting(string name, string sku, decimal price, decimal weight = 0, bool taxable = true)
        {
            return new Product(new ProductVariant(name, sku, price) { Weight = weight, Taxable = taxable });
        }

        public static ProductDisplay MockProductDisplayForInserting()
        {
            return new ProductDisplay()
            {
                Name = ProductItemName(),
                Available = true,
                Barcode = "Barcode1",
                CostOfGoods = 0,
                Download = false,
                Length = 0,
                Height = 0,
                Width = 0,
                SalePrice = 0,
                Price = 10M,
                Shippable = true,
                Taxable = true,
                TrackInventory = false,
                OutOfStockPurchase = false,
                Sku = Guid.NewGuid().ToString(),
                ProductOptions = new List<ProductOptionDisplay>
                {
                    new ProductOptionDisplay()
                    {
                        Key = Guid.Empty,
                        Name = "Color",
                        Required = true,
                        Choices = new List<ProductAttributeDisplay>
                        {
                            new ProductAttributeDisplay()
                            {
                                Key = Guid.Empty,
                                Name = "Blue",
                                Sku = "blue",
                                SortOrder = 0
                            },
                            new ProductAttributeDisplay()
                            {
                                Key = Guid.Empty,
                                Name = "Red",
                                Sku = "red",
                                SortOrder = 1
                            },
                            new ProductAttributeDisplay()
                            {
                                Key = Guid.Empty,
                                Name = "Green",
                                Sku = "green",
                                SortOrder = 2
                            }
                        },
                        SortOrder = 0
                    },
                    new ProductOptionDisplay()
                    {
                        Key = Guid.Empty,
                        Name = "Size",
                        Required = true,
                        Choices = new List<ProductAttributeDisplay>
                        {
                            new ProductAttributeDisplay()
                            {
                                Key = Guid.Empty,
                                Name = "Small",
                                Sku = "small",
                                SortOrder = 0
                            },
                            new ProductAttributeDisplay()
                            {
                                Key = Guid.Empty,
                                Name = "Medium",
                                Sku = "medium",
                                SortOrder = 1
                            },
                            new ProductAttributeDisplay()
                            {
                                Key = Guid.Empty,
                                Name = "Large",
                                Sku = "large",
                                SortOrder = 2
                            }
                        },
                        SortOrder = 1
                    }

                }
            };
        }
    }
}
