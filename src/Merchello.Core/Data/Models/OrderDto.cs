namespace Merchello.Core.Data.Models
{
    using System;
    using System.Collections.Generic;

    internal sealed class OrderDto
    {
        public OrderDto()
        {
            this.MerchOrderItem = new HashSet<OrderItemDto>();
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

        public OrderIndexDto OrderIndexDto { get; set; }

        public ICollection<OrderItemDto> MerchOrderItem { get; set; }

        public InvoiceDto InvoiceDtoKeyNavigation { get; set; }

        public OrderStatusDto OrderStatusDtoKeyNavigation { get; set; }
    }
}