namespace Merchello.Core.Models
{
    

    /// <summary>
    /// Represents a province used in taxation.
    /// </summary>
    public interface ITaxProvince : IProvince
    {
        /// <summary>
        /// Gets or sets the percentage rate adjustment to the tax rate
        /// </summary>
        
        decimal PercentAdjustment { get; set; }
    }
}