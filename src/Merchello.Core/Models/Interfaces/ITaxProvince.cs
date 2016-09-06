namespace Merchello.Core.Models
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines a Tax Province
    /// </summary>
    public interface ITaxProvince : IProvince
    {
        /// <summary>
        /// Gets or sets the percentage rate adjustment to the tax rate
        /// </summary>
        [DataMember]
        decimal PercentAdjustment { get; set; }
    }
}