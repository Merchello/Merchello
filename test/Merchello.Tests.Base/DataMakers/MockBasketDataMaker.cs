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
        public static ICustomerRegistry AnonymousBasket(IAnonymousCustomer anonymous, CustomerRegistryType customerRegistryType)
        {
            var customerRegistry =  new CustomerRegistry(anonymous.Key, customerRegistryType)
            {
                Id = 1,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now
            };

           customerRegistry.ResetDirtyProperties();

            return customerRegistry;

        }

        public static ICustomerRegistry ConsumerBasketForInserting(IConsumer consumer, CustomerRegistryType customerRegistryType)
        {
            return new CustomerRegistry(consumer.Key, customerRegistryType)
            {
                ConsumerKey = consumer.Key
            };
        }

    }
}
