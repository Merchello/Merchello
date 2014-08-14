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
    public class MockCustomerAddressDataMaker : MockDataMakerBase
    {
        
        public static ICustomerAddress CustomerAddressForInserting(Guid customerKey)
        {
            // this won't work for integration tests because of the database constraint.

            var address = new CustomerAddress(customerKey)
                {
                    Label = "Home",
                    Address1 = "111 Somewhere",
                    AddressTypeFieldKey = new AddressTypeField().Shipping.TypeKey,
                    Company = "Demo Co.",
                    Locality = "Seattle",
                    Region = "WA",
                    PostalCode = "99701",
                    CountryCode = "US"
                };

            address.ResetDirtyProperties();

            return address;

        }

        public static ICustomerAddress MindflyAddressForInserting(Guid customerKey)
        {
            var address = new CustomerAddress(customerKey)
                {
                    Label = "Mindfly",
                    Address1 = "114 W. Magnolia St.",
                    Address2 = "Suite 300",
                    Locality = "Bellingham",
                    Region = "WA",
                    CountryCode = "US",
                    PostalCode = "98225",
                    Phone = "555-555-5555"
                };

            address.ResetDirtyProperties();

            return address;
        }


        public static ICustomerAddress RandomAddress(ICustomer customer, string label)
        {
            var addresses = FakeAddresses().ToArray();
            var index = NoWhammyStop.Next(addresses.Count());
            return addresses[index].MakeCustomerAddress(customer, label);
        }


        public static IEnumerable<ICustomerAddress> AddressCollectionForInserting(ICustomer customer, string label, int count)
        {
            for (var i = 0; i < count; i++) yield return RandomAddress(customer, label);
        }


    }
}
