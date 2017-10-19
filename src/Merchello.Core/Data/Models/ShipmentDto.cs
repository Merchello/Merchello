namespace Merchello.Core.Data.Models
{
    using System;
    using System.Collections.Generic;

    internal sealed class ShipmentDto
    {
        public ShipmentDto()
        {
            this.MerchOrderItem = new HashSet<OrderItemDto>();
        }

        public Guid Pk { get; set; }

        public string ShipmentNumberPrefix { get; set; }

        public int ShipmentNumber { get; set; }

        public Guid ShipmentStatusKey { get; set; }

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

        public string Phone { get; set; }

        public string Email { get; set; }

        public Guid? ShipMethodKey { get; set; }

        public Guid VersionKey { get; set; }

        public string Carrier { get; set; }

        public string TrackingCode { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public string TrackingUrl { get; set; }

        public ICollection<OrderItemDto> MerchOrderItem { get; set; }

        public ShipMethodDto ShipMethodDtoKeyNavigation { get; set; }

        public ShipmentStatusDto ShipmentStatusDtoKeyNavigation { get; set; }
    }
}