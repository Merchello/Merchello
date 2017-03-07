using System;
using Merchello.Core;
using Merchello.Core.Models;

namespace Merchello.Tests.Base.DataMakers
{
    public class MockOrderStatusMaker
    {
        public static IOrderStatus OrderStatusNotFulfilledMock()
        {
            return new OrderStatus()
                {
                    Key = Constants.OrderStatus.NotFulfilled,
                    Alias = "notfulfilled",
                    Name = "Not Fulfilled",
                    Active = true,
                    Reportable = true,
                    SortOrder = 1,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now

                };
        }

        public static IOrderStatus OrderStatusFulfilledMock()
        {
            return new OrderStatus()
                {
                    Key = Constants.OrderStatus.Fulfilled,
                    Alias = "fulfilled",
                    Name = "Fulfilled",
                    Active = true,
                    Reportable = true,
                    SortOrder = 3,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now

                };
        }
    }
}