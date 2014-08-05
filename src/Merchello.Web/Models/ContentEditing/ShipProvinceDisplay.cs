namespace Merchello.Web.Models.ContentEditing
{
    using Merchello.Core;

    /// <summary>
    /// The ship province display.
    /// </summary>
    public class ShipProvinceDisplay : ProvinceDisplay
    {
        /// <summary>
        /// Gets or sets a value indicating whether allow shipping.
        /// </summary>
        public bool AllowShipping { get; set; }

        /// <summary>
        /// Gets or sets the rate adjustment.
        /// </summary>
        public decimal RateAdjustment { get; set; }

        /// <summary>
        /// Gets or sets the rate adjustment type.
        /// </summary>
        public RateAdjustmentType RateAdjustmentType { get; set; }
    }
}
