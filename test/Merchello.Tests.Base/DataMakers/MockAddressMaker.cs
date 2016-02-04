namespace Merchello.Tests.Base.DataMakers
{
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Models;

    public class MockAddressMaker : MockDataMakerBase
    {

        public static IAddress GetAddress(string countryCode = "")
        {
            var addresses = string.IsNullOrEmpty(countryCode)
                                ? FakeAddresses().ToArray()
                                : FakeAddresses().Where(x => x.CountryCode == countryCode).ToArray();
            if (!addresses.Any()) return null;

            var index = NoWhammyStop.Next(addresses.Count());
            return addresses[index];
        }

        public static IEnumerable<IAddress> GetAddresses(int count, string countryCode = "")
        {
            for (var i = 0; i < count; i++)
            {
                yield return GetAddress(countryCode);
            }
        } 
    }
}