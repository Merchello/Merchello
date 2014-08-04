namespace Merchello.Tests.Avalara.Integration.TestBase.Mocks
{
    using System;

    using Merchello.Core.Models;
    using Merchello.Core.Models.EntityBase;

    public class ShipmentMock : Entity, IShipment
    {
        public ShipmentMock(IAddress origin, IAddress destination, LineItemCollection items)
        {
            ShippedDate = DateTime.Now;
            FromOrganization = origin.Organization;
            FromName = origin.Name;
            FromAddress1 = origin.Address1;
            FromAddress2 = origin.Address2;
            FromLocality = origin.Locality;
            FromRegion = origin.Region;
            FromPostalCode = origin.PostalCode;
            FromCountryCode = origin.CountryCode;
            FromIsCommercial = origin.IsCommercial;
            ToOrganization = destination.Organization;
            ToName = destination.Name;
            ToAddress1 = destination.Address1;
            ToAddress2 = destination.Address2;
            ToLocality = destination.Locality;
            ToRegion = destination.Region;
            ToPostalCode = destination.PostalCode;
            ToCountryCode = destination.CountryCode;
            ToIsCommercial = destination.IsCommercial;

            Phone = destination.Phone;
            Email = destination.Email;

            Items = items;
        }

        public Guid VersionKey { get; set; }

        public LineItemCollection Items { get; private set; }

        public DateTime ShippedDate { get; set; }

        public string FromOrganization { get; set; }

        public string FromName { get; set; }

        public string FromAddress1 { get; set; }

        public string FromAddress2 { get; set; }

        public string FromLocality { get; set; }

        public string FromRegion { get; set; }

        public string FromPostalCode { get; set; }

        public string FromCountryCode { get; set; }

        public bool FromIsCommercial { get; set; }

        public string ToOrganization { get; set; }

        public string ToName { get; set; }

        public string ToAddress1 { get; set; }

        public string ToAddress2 { get; set; }

        public string ToLocality { get; set; }

        public string ToRegion { get; set; }

        public string ToPostalCode { get; set; }

        public string ToCountryCode { get; set; }

        public bool ToIsCommercial { get; set; }

        public Guid? ShipMethodKey { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Carrier { get; set; }

        public string TrackingCode { get; set; }
    }
}