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
        public static ICustomerItemRegister AnonymousBasket(IAnonymousCustomer anonymous, CustomerItemRegisterType customerItemRegisterType)
        {
            var customerRegistry =  new CustomerItemRegister(anonymous.Key, customerItemRegisterType)
            {
                Id = 1,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now
            };

           customerRegistry.ResetDirtyProperties();

            return customerRegistry;

        }

        public static ICustomerItemRegister ConsumerBasketForInserting(IConsumer consumer, CustomerItemRegisterType customerItemRegisterType)
        {
            return new CustomerItemRegister(consumer.Key, customerItemRegisterType)
            {
                ConsumerKey = consumer.Key
            };
        }

    }
}
