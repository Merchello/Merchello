using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Models.TypeFields;

namespace Merchello.Tests.Base.DataMakers
{
    internal class AddressMock : IAddress
    {
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Locality { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string CountryCode { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Organization { get; set; }

        public bool IsCommercial { get; set; }

        
    }

    internal static class AddressTestHelperExtensions
    {

        public static ICustomerAddress MakeCustomerAddress(this IAddress address, ICustomer customer, string label)
        {
            return new CustomerAddress(customer.Key)
            {
                Label = label,
                FullName = string.Format("{0} {1}", customer.FirstName, customer.LastName).Trim(),
                Address1 = address.Address1,
                Address2 = address.Address2,
                Locality = address.Locality,
                Region = address.Region,
                PostalCode = address.PostalCode,
                CountryCode = address.CountryCode,
                AddressTypeFieldKey = EnumTypeFieldConverter.Address.GetTypeField(AddressType.Shipping).TypeKey
            };
        }

        public static IWarehouse MakeWarehouse(this IAddress address)
        {
            return new Warehouse()
            {
                Name = address.Name,
                Address1 = address.Address1,
                Address2 = address.Address2,
                Locality = address.Locality,
                Region = address.Region,
                PostalCode = address.PostalCode,
                CountryCode = address.CountryCode
            };
        }    
    }

}