using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Merchello.Core.Configuration;
using Merchello.Core.Configuration.Outline;
using Merchello.Core.Models;

namespace Merchello.Core.Services
{
    public class SettingsService : ISettingsService
    {

        private readonly static ConcurrentDictionary<string, IEnumerable<IProvince>> RegionProvinceCache = new ConcurrentDictionary<string, IEnumerable<IProvince>>();

        public SettingsService()
        {
            
            if (!RegionProvinceCache.IsEmpty) return;

            foreach (RegionElement region in MerchelloConfiguration.Current.Section.RegionalProvinces)
            {
                CacheRegion(region.Code, (from ProvinceElement pe in region.ProvincesConfiguration
                    select new Province(pe.Code, pe.Name)).Cast<IProvince>().ToArray());
            }
        }

        private static void CacheRegion(string code, IProvince[] provinces)
        {
            RegionProvinceCache.AddOrUpdate(code, provinces, (x, y) => provinces);
        }
                

        /// <summary>
        /// Returns the <see cref="CountryBase" /> for the country code passed.
        /// </summary>
        /// <param name="countryCode">The two letter ISO Region code (country code)</param>
        /// <returns><see cref="RegionInfo"/> for the country corresponding the the country code passed</returns>
        public ICountry GetCountryByCode(string countryCode)
        {
            return new Country(countryCode, GetProvincesByCountryCode(countryCode))
            {
                ProvinceLabel = GetProvinceLabelForCountry(countryCode)
            };
        }

        /// <summary>
        /// Returns a Region collection for all countries
        /// </summary>
        /// <returns>A collection of <see cref="RegionInfo"/></returns>
        public IEnumerable<ICountry> GetAllCountries()
        {
            return CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                .Select(culture => new RegionInfo(culture.Name))
                .Select(ri => GetCountryByCode(ri.TwoLetterISORegionName));
        }

        /// <summary>
        /// Returns a Region collection for all countries excluding codes passed
        /// </summary>
        /// <param name="excludeCountryCodes">A collection of country codes to exclude from the result set</param>
        /// <returns>A collection of <see cref="RegionInfo"/></returns>
        public IEnumerable<ICountry> GetAllCountries(string[] excludeCountryCodes)
        {
            return GetAllCountries().Where(x => !excludeCountryCodes.Contains(x.CountryCode));
        }

        /// <summary>
        /// True/false indicating whether or not the region has provinces configured in the Merchello.config file
        /// </summary>
        /// <param name="countryCode">The two letter ISO Region code (country code)</param>
        /// <returns></returns>
        private bool CountryHasProvinces(string countryCode)
        {
            return RegionProvinceCache.ContainsKey(countryCode);
        }

        /// <summary>
        /// Returns the province label from the configuration file
        /// </summary>
        /// <param name="countryCode">The two letter ISO Region code</param>
        /// <returns></returns>
        private string GetProvinceLabelForCountry(string countryCode)
        {
            return CountryHasProvinces(countryCode)
                ? MerchelloConfiguration.Current.Section.RegionalProvinces[countryCode].ProvinceLabel
                : string.Empty;
        }

        /// <summary>
        /// Returns a collection of <see cref="IProvince"/> given a region code
        /// </summary>
        /// <param name="countryCode">The two letter ISO Region code (country code)</param>
        /// <returns>A collection of <see cref="IProvince"/></returns>
        private IEnumerable<IProvince> GetProvincesByCountryCode(string countryCode)
        {
            return CountryHasProvinces(countryCode) ?
                RegionProvinceCache[countryCode] :
                new List<IProvince>();
        }
    }
}