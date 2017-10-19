namespace Merchello.Core.Data.Models
{
    using System;
    using System.Collections.Generic;

    internal sealed class NotificationMethodDto
    {
        public NotificationMethodDto()
        {
            this.MerchNotificationMessage = new HashSet<NotificationMessageDto>();
        }

        public Guid Pk { get; set; }

        public Guid ProviderKey { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ServiceCode { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public ICollection<NotificationMessageDto> MerchNotificationMessage { get; set; }

        public GatewayProviderSettingsDto ProviderKeyNavigation { get; set; }
    }
}