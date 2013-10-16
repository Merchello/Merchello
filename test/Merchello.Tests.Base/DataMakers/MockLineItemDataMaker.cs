using Merchello.Core;
using Merchello.Core.Models;

namespace Merchello.Tests.Base.DataMakers
{
    public class MockLineItemDataMaker : MockDataMakerBase
    {
        public static IItemCacheLineItem MockItemCacheLineItemForInserting(int containerId)
        {
            return new ItemCacheLineItem(containerId, LineItemType.Product, ProductItemName(), MockSku(), Quanity(), PriceCheck());
        }

        /// <summary>
        /// Represents a product as if it was returned from the database
        /// </summary>
        /// <param name="containerId"></param>
        /// <returns><see cref="ILineItem"/></returns>
        public static IItemCacheLineItem MockItemCacheLineItemComplete(int containerId)
        {
            var lineItem = MockItemCacheLineItemForInserting(containerId);
            lineItem.Id = 111;
            ((LineItemBase)lineItem).AddingEntity();
            lineItem.ResetDirtyProperties();
            return lineItem;
        }
    }
}