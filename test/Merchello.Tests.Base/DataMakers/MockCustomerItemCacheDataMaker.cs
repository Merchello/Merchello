using System;
using Merchello.Core;
using Merchello.Core.Models;


namespace Merchello.Tests.Base.DataMakers
{
    /// <summary>
    /// Helper class to assist in putting together customer item cache data for testing
    /// </summary>
    public class MockCustomerItemCacheDataMaker : MockDataMakerBase
    {
        public static IItemCache AnonymousBasket(IAnonymousCustomer anonymous, ItemCacheType itemCacheType)
        {
            var itemCache =  new ItemCache(anonymous.Key, itemCacheType)
            {
                Key = Guid.NewGuid(),
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now
            };

           itemCache.ResetDirtyProperties();

            return itemCache;

        }

        public static IItemCache ConsumerItemCacheForInserting(ICustomerBase customer, ItemCacheType itemCacheType)
        {
            return new ItemCache(customer.Key, itemCacheType);
        }

    }
}
