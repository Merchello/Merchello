namespace Merchello.Tests.Base.Mocks
{
    using System;

    using Merchello.Core.Models;
    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Models.Interfaces;

    public class ShipMethodMock : Entity, IShipMethod
    {
        public ShipMethodMock()
        {
            this.Surcharge = 0;
            this.Taxable = false;
            this.ServiceCode = "AvaTaxShipMethodMock";
            this.Provinces = new ProvinceCollection<IShipProvince>();
        }

        public string Name { get; set; }

        public Guid ProviderKey {
            get
            {
                return new Guid("F9F2C286-9C20-4A0A-A2AB-5769EECBF866");
            }
        }

        public Guid ShipCountryKey {
            get
            {
                return new Guid("50B9472F-A0AD-435D-8AF4-D11A49567CCE");
            }
        }

        public decimal Surcharge { get; set; }

        public string ServiceCode { get; set; }

        public bool Taxable { get; set; }

        public ProvinceCollection<IShipProvince> Provinces { get; set; }
    }
}