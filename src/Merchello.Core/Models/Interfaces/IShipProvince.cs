using System.Runtime.Serialization;

namespace Merchello.Core.Models.Interfaces
{
    public interface IShipProvince : IProvince
    {
        /// <summary>
        /// True/false indicating whether or not to allow shipping to the province
        /// </summary>
        [DataMember]
        bool AllowShipping { get; set; }

        /// <summary>
        /// Rate adjustment when shipping to this province
        /// </summary>
        [DataMember]
        decimal RateAdjustment { get; set; }
        
        /// <summary>
        /// Defines the type of rate adjustment
        /// </summary>
        [DataMember]
        RateAdjustmentType RateAdjustmentType { get; set; }
    }
}