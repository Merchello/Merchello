using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Models.TypeFields;

namespace Merchello.Tests.Base.DataMakers
{
    internal class AddressMock
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

        public IWarehouse MakeWarehouse()
        {
            return new Warehouse()
            {
                Name = Name,
                Address1 = Address1,
                Address2 = Address2,
                Locality = Locality,
                Region = Region,
                PostalCode = PostalCode
            };
        }
    }
}