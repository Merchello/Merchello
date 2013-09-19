using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Models;
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

        public static IAddress AddressForUpdating()
        {
            var address = AddressForInserting();
            address.Id = 111;
            address.ResetDirtyProperties();
            return address;

        }

        public static IEnumerable<IAddress> AddressListForInserting()
        {
            return new List<IAddress>()
                {
                    new Address(Guid.NewGuid(), "Home")
                        {
                            Address1 = "111 Somewhere",
                            AddressTypeFieldKey = new AddressTypeField().Residential.TypeKey,
                            Company = "Demo Co.",
                            Locality = "Seattle",
                            Region = "WA",
                            PostalCode = "99701",
                            CountryCode = "US"
                        },
                    new Address(Guid.NewGuid(), "Viva")
                        {
                            Address1 = "666 Drifters Highway",
                            AddressTypeFieldKey = new AddressTypeField().Commercial.TypeKey,
                            Company = "Vegas.",
                            Locality = "Las Vegas",
                            Region = "NV",
                            PostalCode = "00122",
                            CountryCode = "US"
                        },
                    new Address(Guid.NewGuid(), "Condo")
                        {
                            Address1 = "12 Hampton Ct.",
                            AddressTypeFieldKey = new AddressTypeField().Residential.TypeKey,
                            Locality = "District of Columbia",
                            Region = "DC",
                            PostalCode = "11111",
                            CountryCode = "US"
                        }

                };


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

        public static IEnumerable<AddressMock> AddressMocks()
        {
            return new List<AddressMock>()
                {
                    new AddressMock()
                        {
                            Name = "Walt Disney World Resort",
                            Locality = "Lake Buena Vista",
                            Region = "FL",
                            PostalCode = "32830",
                            CountryCode = "US"
                        },
                    new AddressMock()
                        {
                            Name = "Rockefeller Center",
                            Address1 = "45 Rockefeller Plz",
                            Locality = "New York",
                            Region = "NY",
                            PostalCode = "10111"
                        },
                    new AddressMock()
                        {
                            Name = "Eiffel Tower",
                            Address1 = "Champs-de-Mars",
                            Locality = "Paris",
                            PostalCode = "75007",
                            CountryCode = "FR"
                        },
                    new AddressMock()
                        {
                            Name = "Buckingham Palace",
                            Address1 = "SW1A 1AA",
                            Locality = "London",
                            CountryCode = "UK"
                        },
                    new AddressMock()
                        {
                            Name = "Space Needle",
                            Address1 = "400 Broad St",
                            Locality = "Seattle",
                            Region = "WA",
                            PostalCode = "98102"
                        },
                    new AddressMock()
                        {
                            Name = "Sydney Opera House",
                            Address1 = "Bennelong Point",
                            Locality = "Sydney",
                            PostalCode = "NSW 2000",
                            CountryCode = "AU"
                        }
                };

        }


        public class AddressMock
        {
            public string Name { get; set; }
            public string Address1 { get; set; }
            public string Address2 { get; set; }
            public string Locality { get; set; }
            public string Region { get; set; }
            public string PostalCode { get; set; }
            public string CountryCode { get; set; }

            public IAddress MakeAddress(ICustomer customer, string label)
            {
                return new Address(customer.Key, label)
                    {
                        FullName = string.Format("{0} {1}", customer.FirstName, customer.LastName).Trim(),
                        Address1 = this.Address1,
                        Address2 = this.Address2,
                        Locality = this.Locality,
                        Region = this.Region,
                        PostalCode = this.PostalCode,
                        CountryCode = this.CountryCode,
                        AddressTypeFieldKey = EnumTypeFieldConverter.Address().GetTypeField(AddressType.Residential).TypeKey
                    };
            }
        }


    }
}
