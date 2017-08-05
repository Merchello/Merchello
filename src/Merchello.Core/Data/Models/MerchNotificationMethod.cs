namespace Merchello.Core.Data.Models
{
    using System;
    using System.Collections.Generic;

    internal sealed partial class MerchNotificationMethod
    {
        public MerchNotificationMethod()
        {
            this.MerchNotificationMessage = new HashSet<MerchNotificationMessage>();
        }

        public Guid Pk { get; set; }

        public Guid ProviderKey { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ServiceCode { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public ICollection<MerchNotificationMessage> MerchNotificationMessage { get; set; }

        public MerchGatewayProviderSettings ProviderKeyNavigation { get; set; }
    }
}