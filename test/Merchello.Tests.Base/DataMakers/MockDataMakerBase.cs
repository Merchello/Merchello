using System;
using System.Linq;

namespace Merchello.Tests.Base.DataMakers
{
    public abstract class MockDataMakerBase
    {
        public static Random NoWhammyStop = new Random();


        public static string MockSku()
        {
            return Guid.NewGuid().ToString().Substring(0, 8);
        }

        protected static decimal GetAmount()
        {
            return NoWhammyStop.Next(1500);
        }

        protected static string ProductItemName()
        {
            var descriptions = new[]
            {
                "Gravy boat", "Sushi", "Chop sticks", "Golf ball", "Paraglider", "Pet rock", "Parachute pants", "Pink Pirates Flag", "Picture of Miguel", "Bar of soap", "1 Kg. Oysters",
                "Maple Syrup", "Spark plug", "Paper cup", "Monitor adapter", "Sun glasses", "Shoe laces", "Fern", "Ghandi Facts and Fiction", "Snorkel", "Box of frogs", "Peanut Butter", "Zip lock bags",
                "Goats milk", "Iguana", "Sheppard's crook", "Pricess Liea Slave Custome", "Refrigerator Perry Collectable Card", "Corn flour", "Organic chocolate", "Laser pistol", "Alien bogey",
                "Harry Potter Speedo", "Golf ball", "Slingshot", "Pepper Spray", "Chicken Feathers", "Slinky", "Toe Nail Clippers", "iPad", "Diaper Service", "Bait",
                "Escalator", "Gold Ring", "Whistle Pop", "Spicy Sauce", "Snails", "Roses", "Razor", "Remote Control", "Frosting", "Camoflage Muscle Shirt",
                "Vanilla Ice Cream", "Popcorn", "Frodo Action Figure", "Used Shoes", "Shih Tzu Grooming Kit", "Yogurt",
                "Oranges", "Straw Hat", "Excalibar", "Box of Cracker Jacks", "Tackle Box",
                "L'Ecole Perigree", "Monster Truck", "Jawa", "Cheetos", "Original Blizzard of Oz LP", "Umbraco TV Subscription",
                "Strudle", "Alarm Clock", "Coffee"
            };

            var index = NoWhammyStop.Next(descriptions.Length);
            return descriptions[index];
        }

        protected static decimal PriceCheck()
        {
            var cost = NoWhammyStop.Next(100);
            return Convert.ToDecimal(cost);
        }

        protected static string SelectRandomString(string[] values)
        {

            var index = NoWhammyStop.Next(values.Count());
            return values.ToArray()[index];
        }
    }
}
