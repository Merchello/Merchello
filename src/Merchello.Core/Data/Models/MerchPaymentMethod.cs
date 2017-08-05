namespace Merchello.Core.Data.Models
{
    using System;
    using System.Collections.Generic;

    internal sealed partial class MerchPaymentMethod
    {
        public MerchPaymentMethod()
        {
            this.MerchPayment = new HashSet<MerchPayment>();
        }

        public Guid Pk { get; set; }

        public Guid ProviderKey { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string PaymentCode { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public ICollection<MerchPayment> MerchPayment { get; set; }

        public MerchGatewayProviderSettings ProviderKeyNavigation { get; set; }
    }
}