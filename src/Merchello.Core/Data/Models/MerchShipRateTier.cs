namespace Merchello.Core.Data.Models
{
    using System;

    internal partial class MerchShipRateTier
    {
        public Guid Pk { get; set; }

        public Guid ShipMethodKey { get; set; }

        public decimal RangeLow { get; set; }

        public decimal RangeHigh { get; set; }

        public decimal Rate { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public virtual MerchShipMethod ShipMethodKeyNavigation { get; set; }
    }
}