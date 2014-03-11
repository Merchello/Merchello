using Merchello.Core;
using System;
using System.Collections.Generic;

namespace Merchello.Web.Models.ContentEditing
{
    public class ShipFixedRateTableDisplay
    {
        public Guid ShipMethodKey { get; set; }
        public IEnumerable<ShipRateTierDisplay> Rows { get; set; }
    }
}
