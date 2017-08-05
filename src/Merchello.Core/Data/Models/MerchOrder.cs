namespace Merchello.Core.Data.Models
{
    using System;
    using System.Collections.Generic;

    internal sealed partial class MerchOrder
    {
        public MerchOrder()
        {
            this.MerchOrderItem = new HashSet<MerchOrderItem>();
        }

        public Guid Pk { get; set; }

        public Guid? InvoiceKey { get; set; }

        public string OrderNumberPrefix { get; set; }

        public int OrderNumber { get; set; }

        public DateTime OrderDate { get; set; }

        public Guid OrderStatusKey { get; set; }

        public Guid VersionKey { get; set; }

        public bool Exported { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public MerchOrderIndex MerchOrderIndex { get; set; }

        public ICollection<MerchOrderItem> MerchOrderItem { get; set; }

        public MerchInvoice InvoiceKeyNavigation { get; set; }

        public MerchOrderStatus OrderStatusKeyNavigation { get; set; }
    }
}