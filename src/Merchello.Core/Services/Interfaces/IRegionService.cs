using System.Collections.Generic;
using System.Globalization;
using Merchello.Core.Models;

namespace Merchello.Core.Services
{
    public interface IRegionService
    {
        /// <summary>
        /// Returns the <see cref="RegionInfo"/> for the country code passed.
        /// </summary>
        /// <param name="regionCode">The two letter ISO Region code (country code)</param>
        /// <returns><see cref="RegionInfo"/> for the country corresponding the the country code passed</returns>
        RegionInfo GetRegionInfoByCode(string regionCode);
        
        /// <summary>
        /// Returns a RegionInfo collection for all countries
        /// </summary>
        /// <returns>A collection of <see cref="RegionInfo"/></returns>
        IEnumerable<RegionInfo> GetRegionInfoList();

        /// <summary>
        /// Returns a RegionInfo collection for all countries excluding codes passed
        /// </summary>
        /// <param name="excludeCodes">A collection of country codes to exclude from the result set</param>
        /// <returns>A collection of <see cref="RegionInfo"/></returns>
        IEnumerable<RegionInfo> GetRegionInfoList(string[] excludeCodes);

        /// <summary>
        /// True/false indicating whether or not the region has provinces configured in the Merchello.config file
        /// </summary>
        /// <param name="regionCode">The two letter ISO Region code (country code)</param>
        /// <returns></returns>
        bool RegionHasProvinces(string regionCode);

        /// <summary>
        /// Returns a collection of <see cref="IProvince"/> given a region code
        /// </summary>
        /// <param name="regionCode">The two letter ISO Region code (country code)</param>
        /// <returns>A collection of <see cref="IProvince"/></returns>
        IEnumerable<IProvince> GetProvincesByCode(string regionCode);
    }
}