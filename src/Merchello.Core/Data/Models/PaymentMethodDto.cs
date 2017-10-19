namespace Merchello.Core.Data.Models
{
    using System;
    using System.Collections.Generic;

    internal sealed class PaymentMethodDto
    {
        public PaymentMethodDto()
        {
            this.MerchPayment = new HashSet<PaymentDto>();
        }

        public Guid Pk { get; set; }

        public Guid ProviderKey { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string PaymentCode { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public ICollection<PaymentDto> MerchPayment { get; set; }

        public GatewayProviderSettingsDto ProviderKeyNavigation { get; set; }
    }
}