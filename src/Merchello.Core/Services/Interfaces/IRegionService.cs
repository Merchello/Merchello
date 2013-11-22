using System.Collections.Generic;
using System.Globalization;
using Merchello.Core.Models;

namespace Merchello.Core.Services
{
    public interface IRegionService
    {
        /// <summary>
        /// Returns the <see cref="IWarehouseCountry"/> for the country code passed.
        /// </summary>
        /// <param name="regionCode">The two letter ISO Region code (country code)</param>
        /// <returns><see cref="RegionInfo"/> for the country corresponding the the country code passed</returns>
        IWarehouseCountry GetRegionByCode(string regionCode);

        /// <summary>
        /// Returns a Region collection for all countries
        /// </summary>
        /// <returns>A collection of <see cref="IWarehouseCountry"/></returns>
        IEnumerable<IWarehouseCountry> GetAllRegions();

        /// <summary>
        /// Returns a RegionInfo collection for all countries excluding codes passed
        /// </summary>
        /// <param name="excludeCodes">A collection of country codes to exclude from the result set</param>
        /// <returns>A collection of <see cref="RegionInfo"/></returns>
        IEnumerable<IWarehouseCountry> GetAllRegions(string[] excludeCodes);

    }
}