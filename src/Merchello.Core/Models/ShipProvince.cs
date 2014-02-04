using System;
using System.Runtime.Serialization;
using Merchello.Core.Models.Interfaces;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Represents a province from a shipping context
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    internal class ShipProvince : Province, IShipProvince
    {
        public ShipProvince(string code, string name)
            : base(code, name)
        {
            AllowShipping = true;
            RateAdjustment = 0;
            RateAdjustmentType = RateAdjustmentType.Numeric;
        }

        /// <summary>
        /// True/false indicating whether or not to allow shipping to the province
        /// </summary>
        [DataMember]
        public bool AllowShipping { get; set; }

        /// <summary>
        /// Price adjustment when shipping to this province
        /// </summary>
        [DataMember]
        public decimal RateAdjustment { get; set; }

        /// <summary>
        /// Defines whether a rate adjustment should be a fixed numeric adjustment or calculated as a percentage
        /// </summary>
        [DataMember]
        public RateAdjustmentType RateAdjustmentType { get; set; }
    }
}