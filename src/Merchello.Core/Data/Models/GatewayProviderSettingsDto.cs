namespace Merchello.Core.Data.Models
{
    using System;
    using System.Collections.Generic;

    internal sealed partial class GatewayProviderSettingsDto
    {
        public GatewayProviderSettingsDto()
        {
            this.MerchNotificationMethod = new HashSet<NotificationMethodDto>();
            this.MerchPaymentMethod = new HashSet<PaymentMethodDto>();
            this.MerchShipMethod = new HashSet<ShipMethodDto>();
            this.MerchTaxMethod = new HashSet<TaxMethodDto>();
        }

        public Guid Pk { get; set; }

        public Guid ProviderTfKey { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ExtendedData { get; set; }

        public bool EncryptExtendedData { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public ICollection<NotificationMethodDto> MerchNotificationMethod { get; set; }

        public ICollection<PaymentMethodDto> MerchPaymentMethod { get; set; }

        public ICollection<ShipMethodDto> MerchShipMethod { get; set; }

        public ICollection<TaxMethodDto> MerchTaxMethod { get; set; }
    }
}