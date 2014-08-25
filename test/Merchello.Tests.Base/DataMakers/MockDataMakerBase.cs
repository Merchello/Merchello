using System;
using System.Collections.Generic;
using System.Linq;

namespace Merchello.Tests.Base.DataMakers
{
    using Merchello.Core.Models;

    public abstract class MockDataMakerBase
    {
        public static Random NoWhammyStop = new Random();


        public static string MockSku()
        {
            return Guid.NewGuid().ToString().Substring(0, 8);
        }

        public static decimal GetAmount()
        {
            return NoWhammyStop.Next(150);
        }

        public static decimal GetWeight()
        {
            return NoWhammyStop.Next(8);
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

        protected static int Quanity()
        {
            return NoWhammyStop.Next(25);
        }

        internal static IEnumerable<IAddress> FakeAddresses()
        {
            return new List<IAddress>()
                {
                    new Address()
                        {
                            Name = "Walt Disney World Resort",
                            Locality = "Lake Buena Vista",
                            Region = "FL",
                            PostalCode = "32830",
                            CountryCode = "US"
                        },
                    new Address()
                        {
                            Name = "Rockefeller Center",
                            Address1 = "45 Rockefeller Plz",
                            Locality = "New York",
                            Region = "NY",
                            CountryCode = "US",
                            PostalCode = "10111"
                        },
                    new Address()
                        {
                            Name = "Eiffel Tower",
                            Address1 = "Champs-de-Mars",
                            Locality = "Paris",
                            PostalCode = "75007",
                            CountryCode = "FR"
                        },
                    new Address()
                        {
                            Name = "Buckingham Palace",
                            Address1 = "SW1A 1AA",
                            Locality = "London",
                            CountryCode = "UK"
                        },
                    new Address()
                        {
                            Name = "Space Needle",
                            Address1 = "400 Broad St",
                            Locality = "Seattle",
                            Region = "WA",
                            PostalCode = "98102",
                            CountryCode = "US"
                        },
                    new Address()
                        {
                            Name = "Sydney Opera House",
                            Address1 = "Bennelong Point",
                            Locality = "Sydney",
                            PostalCode = "NSW 2000",
                            CountryCode = "AU"
                        }
                };
        }
    }
}
