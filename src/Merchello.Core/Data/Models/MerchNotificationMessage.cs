namespace Merchello.Core.Data.Models
{
    using System;

    internal partial class MerchNotificationMessage
    {
        public Guid Pk { get; set; }

        public Guid MethodKey { get; set; }

        public Guid? MonitorKey { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string FromAddress { get; set; }

        public string ReplyTo { get; set; }

        public string BodyText { get; set; }

        public int MaxLength { get; set; }

        public bool BodyTextIsFilePath { get; set; }

        public string Recipients { get; set; }

        public bool SendToCustomer { get; set; }

        public bool Disabled { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public virtual MerchNotificationMethod MethodKeyNavigation { get; set; }
    }
}