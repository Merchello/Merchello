using System;
using Merchello.Core.Models;

namespace Merchello.Tests.Base.DataMakers
{
    public class MockProductDataMaker : MockDataMakerBase
    {

        public static IProduct MockProductForInserting()
        {
            return MockProductForInserting(MockSku(), ProductName(), PriceCheck());
        }

        public static IProduct MockProductForInserting(string sku, string name, decimal price)
        {
            return new Product()
            {
                Sku = sku,
                Name = name,
                Price = price,
                CostOfGoods = null,
                SalePrice = null,
                Weight = null,
                Length = null,
                Width = null,
                Height = null,
                Taxable = true,
                Shippable = false,
                Download = false,
                Template = false
            };
        }

        public static string MockSku()
        {
            return Guid.NewGuid().ToString().Substring(0, 8);
        }

        private static decimal PriceCheck()
        {
            var cost = NoWhammyStop.Next(100);
            return Convert.ToDecimal(cost);
        }

        private static string ProductName()
        {
            var names = new[]
                {
                    "Golf ball", "Slingshot", "Pepper Spray", "Chicken Feathers", "Slinky", "Toe Nail Clippers", "iPad",
                    "Escalator", "Gold Ring", "Whistle Pop", "Spicy Sauce", "Snails", "Roses", "Razor", "Remote Control",
                    "Vanilla Ice Cream", "Popcorn", "Frodo Action Figure", "Used Shoes", "Shih Tzu Grooming Kit",
                    "Oranges", "Straw Hat", "Excalibar", "Box of Cracker Jacks",
                    "L'Ecole Perigree", "Monster Truck", "Jawa", "Cheetos", "Original Blizzard of Oz LP", "Umbraco TV Subscription",
                    "Strudle", "Alarm Clock", "Coffee"
                };
            return SelectRandomString(names);
        }
    }
}
