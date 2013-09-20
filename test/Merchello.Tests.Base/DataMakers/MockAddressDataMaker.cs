using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.TypeFields;

namespace Merchello.Tests.Base.DataMakers
{
    /// <summary>
    /// Helper class to assist in putting together address data for testing
    /// </summary>
    public class MockAddressDataMaker : MockDataMakerBase
    {
        
        public static IAddress AddressForInserting()
        {
            // this won't work for integration tests because of the database constraint.

            var address = new Address(Guid.NewGuid(), "Home")
                {
                    Address1 = "111 Somewhere",
                    AddressTypeFieldKey = new AddressTypeField().Residential.TypeKey,
                    Company = "Demo Co.",
                    Locality = "Seattle",
                    Region = "WA",
                    PostalCode = "99701",
                    CountryCode = "US"
                };

            address.ResetDirtyProperties();

            return address;

        }

        public static IAddress MindflyAddressForInserting()
        {
            var address = new Address(Guid.NewGuid(), "Mindfly")
                {
                    Address1 = "114 W. Magnolia St.",
                    Address2 = "Suite 504",
                    Locality = "Bellingham",
                    Region = "WA",
                    CountryCode = "US",
                    PostalCode = "98225",
                    Phone = "555-555-5555"
                };

            address.ResetDirtyProperties();

            return address;
        }


        public static IAddress RandomAddress(ICustomer customer, string label)
        {
            var addresses = AddressMocks().ToArray();
            var index = NoWhammyStop.Next(addresses.Count());
            return addresses[index].MakeAddress(customer, label);
        }


        public static IEnumerable<IAddress> AddressCollectionForInserting(ICustomer customer, string label, int count)
        {
            for (var i = 0; i < count; i++) yield return RandomAddress(customer, label);
        }


    }
}
