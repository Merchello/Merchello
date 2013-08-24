using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Models;
using Merchello.Core.Models.TypeFields;

namespace Merchello.Tests.Base.Data
{
    public class AddressData
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

    }
}
