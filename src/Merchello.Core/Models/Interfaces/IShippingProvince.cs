using System.Runtime.Serialization;

namespace Merchello.Core.Models.Interfaces
{
    public interface IShippingProvince : IProvince
    {
        /// <summary>
        /// True/false indicating whether or not to allow shipping to the province
        /// </summary>
        [DataMember]
        bool ShipTo { get; set; }

        /// <summary>
        /// Price adjustment when shipping to this province
        /// </summary>
        [DataMember]
        decimal ShippingAdjustment { get; set; }

    }
}