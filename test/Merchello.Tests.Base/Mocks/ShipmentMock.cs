namespace Merchello.Tests.Base.Mocks
{
    using System;

    using Merchello.Core.Models;
    using Merchello.Core.Models.EntityBase;

    public class ShipmentMock : Entity, IShipment
    {
        public ShipmentMock(IAddress origin, IAddress destination, LineItemCollection items)
        {

            ShipmentNumber = 1;
            ShipmentStatus = new ShipmentStatusMock();

            this.ShippedDate = DateTime.Now;
            this.FromOrganization = origin.Organization;
            this.FromName = origin.Name;
            this.FromAddress1 = origin.Address1;
            this.FromAddress2 = origin.Address2;
            this.FromLocality = origin.Locality;
            this.FromRegion = origin.Region;
            this.FromPostalCode = origin.PostalCode;
            this.FromCountryCode = origin.CountryCode;
            this.FromIsCommercial = origin.IsCommercial;
            this.ToOrganization = destination.Organization;
            this.ToName = destination.Name;
            this.ToAddress1 = destination.Address1;
            this.ToAddress2 = destination.Address2;
            this.ToLocality = destination.Locality;
            this.ToRegion = destination.Region;
            this.ToPostalCode = destination.PostalCode;
            this.ToCountryCode = destination.CountryCode;
            this.ToIsCommercial = destination.IsCommercial;

            this.Phone = destination.Phone;
            this.Email = destination.Email;

            this.Items = items;
        }

        public Guid VersionKey { get; set; }

        public LineItemCollection Items { get; private set; }

        public string ShipmentNumberPrefix { get; set; }
        public int ShipmentNumber { get; set; }
        public Guid ShipmentStatusKey { get { return ShipmentStatus.Key; } }
        public IShipmentStatus ShipmentStatus { get; set; }
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