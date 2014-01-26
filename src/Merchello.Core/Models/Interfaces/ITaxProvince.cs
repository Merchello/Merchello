namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a Tax Province
    /// </summary>
    public interface ITaxProvince : IProvince
    {
        /// <summary>
        /// The percentage rate adjustment to the tax rate
        /// </summary>
        decimal PercentRateAdjustment { get; set; }
    }
}