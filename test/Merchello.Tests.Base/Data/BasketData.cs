using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core;
using Merchello.Core.Models;

namespace Merchello.Tests.Base.Data
{
    public class BasketData
    {
        public static IBasket AnonymousBasket(BasketType basketType)
        {
            var anonymous = CustomerData.AnonymousCustomerMock();
           

            var basket =  new Basket()
            {
                Id = 1,
                ConsumerKey = anonymous.Key,
                BasketType = basketType,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now
            };

           basket.ResetDirtyProperties();

            return basket;

        }

        public static IBasket AnonymousBasketForInserting(BasketType basketType)
        {
            var anonymous = CustomerData.AnonymousCustomerMock();
            return new Basket()
            {
                ConsumerKey = anonymous.Key,
                BasketType = basketType
            };
        }

    }
}
