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
        public static ICustomerItemCache AnonymousBasket(IAnonymousCustomer anonymous, ItemCacheType itemCacheType)
        {
            var itemCache =  new CustomerItemCache(anonymous.Key, itemCacheType)
            {
                Id = 1,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now
            };

           itemCache.ResetDirtyProperties();

            return itemCache;

        }

        public static ICustomerItemCache ConsumerItemCacheForInserting(ICustomerBase customer, ItemCacheType itemCacheType)
        {
            return new CustomerItemCache(customer.Key, itemCacheType)
            {
                CustomerKey = customer.Key
            };
        }

    }
}
