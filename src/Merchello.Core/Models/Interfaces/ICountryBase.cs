namespace Merchello.Core.Models
{
    using System;
    using System.Globalization;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Represents a region
    /// </summary>
    [Obsolete]
    public interface ICountryBase : ICountry 
    {
             
        /// <summary>
        /// The <see cref="System.Globalization.RegionInfo"/> associated with the Region
        /// </summary>
        RegionInfo RegionInfo { get; } 
    }
}