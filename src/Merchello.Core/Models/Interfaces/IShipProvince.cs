namespace Merchello.Core.Models.Interfaces
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines a shipping province.
    /// </summary>
    public interface IShipProvince : IProvince
    {
        /// <summary>
        /// Gets or sets a value indicating whether or not to allow shipping to the province
        /// </summary>
        [DataMember]
        bool AllowShipping { get; set; }

        /// <summary>
        /// Gets or sets the rate adjustment when shipping to this province
        /// </summary>
        [DataMember]
        decimal RateAdjustment { get; set; }
        
        /// <summary>
        /// Gets or sets the type of rate adjustment
        /// </summary>
        [DataMember]
        RateAdjustmentType RateAdjustmentType { get; set; }
    }
}