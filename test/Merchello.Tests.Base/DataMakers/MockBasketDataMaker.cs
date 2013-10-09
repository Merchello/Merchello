using System;
using Merchello.Core;
using Merchello.Core.Models;


namespace Merchello.Tests.Base.DataMakers
{
    /// <summary>
    /// Helper class to assist in putting together basket data for testing
    /// </summary>
    public class MockBasketDataMaker : MockDataMakerBase
    {
        public static ICustomerItemCache AnonymousBasket(IAnonymousCustomer anonymous, CustomerItemCacheType customerItemCacheType)
        {
            var customerRegistry =  new CustomerItemCache(anonymous.Key, customerItemCacheType)
            {
                Id = 1,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now
            };

           customerRegistry.ResetDirtyProperties();

            return customerRegistry;

        }

        public static ICustomerItemCache ConsumerBasketForInserting(IConsumer consumer, CustomerItemCacheType customerItemCacheType)
        {
            return new CustomerItemCache(consumer.Key, customerItemCacheType)
            {
                ConsumerKey = consumer.Key
            };
        }

    }
}
