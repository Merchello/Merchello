using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Models;

namespace Merchello.Tests.Base.Data
{
    public class AddressData
    {

        public static IAddress AddressForInserting()
        {
            var address = new Address(0, new Guid(), string.Empty)
            {
                Id = 0, 
                CustomerPk = new Guid(), 
                Label = "Billing"
            };

            address.ResetDirtyProperties();

            return address;
        }

        public static IAddress AddressForUpdating()
        {
            var address = AddressData.AddressForInserting();
            address.Id = 0;
            address.ResetDirtyProperties();
            return address;

        }

        public static IEnumerable<IAddress> AddressListForInserting()
        {
            return new List<IAddress>()
                {
                    new Address(0, new Guid(), null)
                        {
                            Id = 0, 
                            CustomerPk = new Guid(), 
                            Label = "Billing"
                        },
                    new Address(0, new Guid(), null)
                        {
                            Id = 0, 
                            CustomerPk = new Guid(), 
                            Label = "Shipping"
                        },
                     new Address(0, new Guid(), null)
                        {
                            Id = 0, 
                            CustomerPk = new Guid(), 
                            Label = "Summer Cabin"
                        }

                };


        }

    }
}
