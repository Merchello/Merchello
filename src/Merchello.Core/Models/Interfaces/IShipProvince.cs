namespace Merchello.Core.Models.Interfaces
{
    

    /// <summary>
    /// Represents a shipping province.
    /// </summary>
    public interface IShipProvince : IProvince
    {
        /// <summary>
        /// Gets or sets a value indicating whether or not to allow shipping to the province
        /// </summary>
        
        bool AllowShipping { get; set; }

        /// <summary>
        /// Gets or sets the rate adjustment when shipping to this province
        /// </summary>
        
        decimal RateAdjustment { get; set; }
        
        /// <summary>
        /// Gets or sets the type of rate adjustment
        /// </summary>
        
        //RateAdjustmentType RateAdjustmentType { get; set; }
    }
}