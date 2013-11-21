using System.Runtime.Serialization;

namespace Merchello.Core.Models.Interfaces
{
    public interface ITaxProvince
    {
        /// <summary>
        /// Percentage adjustment for orders shipped to this province
        /// </summary>
        [DataMember]
        decimal PercentAdjustment { get; set; } 
 
    }
}