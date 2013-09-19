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
        public static IBasket AnonymousBasket(IAnonymousCustomer anonymous, BasketType basketType)
        {
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

        public static IBasket AnonymousBasketForInserting(IAnonymousCustomer anonymous, BasketType basketType)
        {
            return new Basket(basketType)
            {
                ConsumerKey = anonymous.Key
            };
        }

    }
}
