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
    internal class ShippingProvince : Province, IShippingProvince
    {
        public ShippingProvince(string code, string name)
            : base(code, name)
        {
            ShipTo = true;
            ShippingAdjustment = 0;
        }

        /// <summary>
        /// True/false indicating whether or not to allow shipping to the province
        /// </summary>
        [DataMember]
        public bool ShipTo { get; set; }

        /// <summary>
        /// Price adjustment when shipping to this province
        /// </summary>
        [DataMember]
        public decimal ShippingAdjustment { get; set; }
    }
}