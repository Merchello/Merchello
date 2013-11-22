using System.Globalization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Represents a region
    /// </summary>
    public interface ICountryBase : ICountry 
    {
             
        /// <summary>
        /// The <see cref="System.Globalization.RegionInfo"/> associated with the Region
        /// </summary>
        RegionInfo RegionInfo { get; } 

    }
}