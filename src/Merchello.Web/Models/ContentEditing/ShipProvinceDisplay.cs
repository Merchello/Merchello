using Merchello.Core;
using System;

namespace Merchello.Web.Models.ContentEditing
{
    public class ShipProvinceDisplay : ProvinceDisplay
    {
        //public string Name { get; set; }
        //public string Code { get; set; }
        public bool AllowShipping { get; set; }
        public decimal RateAdjustment { get; set; }
        public RateAdjustmentType RateAdjustmentType { get; set; }
    }
}
