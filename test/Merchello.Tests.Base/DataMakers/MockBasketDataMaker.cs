using System;
using Merchello.Core;
using Merchello.Core.Models;


namespace Merchello.Tests.Base.DataMakers
{
    public class MockBasketDataMaker : MockDataMakerBase
    {
        public static IBasket AnonymousBasket(BasketType basketType)
        {
            var anonymous = MockCustomerDataMaker.AnonymousCustomerMock();
           

            var basket =  new Basket(basketType)
            {
                Id = 1,
                ConsumerKey = anonymous.Key,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now
            };

           basket.ResetDirtyProperties();

            return basket;

        }

        public static IBasket AnonymousBasketForInserting(BasketType basketType)
        {
            var anonymous = MockCustomerDataMaker.AnonymousCustomerMock();
            return new Basket(basketType)
            {
                ConsumerKey = anonymous.Key
            };
        }

    }
}
