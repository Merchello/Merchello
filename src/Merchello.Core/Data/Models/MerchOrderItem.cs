namespace Merchello.Core.Data.Models
{
    using System;

    internal partial class MerchOrderItem
    {
        public Guid Pk { get; set; }

        public Guid OrderKey { get; set; }

        public Guid? ShipmentKey { get; set; }

        public Guid LineItemTfKey { get; set; }

        public string Sku { get; set; }

        public string Name { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public bool BackOrder { get; set; }

        public bool Exported { get; set; }

        public string ExtendedData { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public virtual MerchOrder OrderKeyNavigation { get; set; }

        public virtual MerchShipment ShipmentKeyNavigation { get; set; }
    }
}