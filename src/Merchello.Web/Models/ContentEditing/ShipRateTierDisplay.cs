using Merchello.Core;
using System;

namespace Merchello.Web.Models.ContentEditing
{
    public class ShipRateTierDisplay
    {
        public Guid Key { get; set; }
        public Guid ShipMethodKey { get; set; }
        public decimal RangeLow { get; set; }
        public decimal RangeHigh { get; set; }
        public decimal Rate { get; set; }
    }
}
