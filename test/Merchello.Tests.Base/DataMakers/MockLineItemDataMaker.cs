using System;
using Merchello.Core;
using Merchello.Core.Models;

namespace Merchello.Tests.Base.DataMakers
{
    public class MockLineItemDataMaker : MockDataMakerBase
    {
        public static IItemCacheLineItem MockItemCacheLineItemForInserting(Guid containerKey)
        {
            return new ItemCacheLineItem(LineItemType.Product, ProductItemName(), MockSku(), Quanity(), PriceCheck())
                {
                    ContainerKey = containerKey
                };
        }

        /// <summary>
        /// Represents a product as if it was returned from the database
        /// </summary>
        /// <param name="containerKey"></param>
        /// <returns><see cref="ILineItem"/></returns>
        public static IItemCacheLineItem MockItemCacheLineItemComplete(Guid containerKey)
        {
            var lineItem = MockItemCacheLineItemForInserting(containerKey);
            lineItem.Key = Guid.NewGuid();
            ((LineItemBase)lineItem).AddingEntity();
            lineItem.ResetDirtyProperties();
            return lineItem;
        }
    }
}