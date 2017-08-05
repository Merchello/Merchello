namespace Merchello.Core.Data.Models
{
    using System;
    using System.Collections.Generic;

    internal sealed partial class MerchGatewayProviderSettings
    {
        public MerchGatewayProviderSettings()
        {
            this.MerchNotificationMethod = new HashSet<MerchNotificationMethod>();
            this.MerchPaymentMethod = new HashSet<MerchPaymentMethod>();
            this.MerchShipMethod = new HashSet<MerchShipMethod>();
            this.MerchTaxMethod = new HashSet<MerchTaxMethod>();
        }

        public Guid Pk { get; set; }

        public Guid ProviderTfKey { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ExtendedData { get; set; }

        public bool EncryptExtendedData { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public ICollection<MerchNotificationMethod> MerchNotificationMethod { get; set; }

        public ICollection<MerchPaymentMethod> MerchPaymentMethod { get; set; }

        public ICollection<MerchShipMethod> MerchShipMethod { get; set; }

        public ICollection<MerchTaxMethod> MerchTaxMethod { get; set; }
    }
}