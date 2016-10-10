namespace Merchello.Core.Models
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents a Tax Province
    /// </summary>
    public interface ITaxProvince : IProvince
    {
        /// <summary>
        /// Gets or sets the percentage rate adjustment to the tax rate
        /// </summary>
        decimal PercentAdjustment { get; set; }
    }
}