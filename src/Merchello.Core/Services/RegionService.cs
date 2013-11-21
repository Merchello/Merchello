using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Merchello.Core.Configuration;
using Merchello.Core.Configuration.Outline;
using Merchello.Core.Models;
using Merchello.Core.Persistence;
using Merchello.Core.Persistence.UnitOfWork;

namespace Merchello.Core.Services
{
    internal class RegionService : IRegionService
    {
        private readonly static ConcurrentDictionary<string, IEnumerable<IProvince>> RegionProvinceCache = new ConcurrentDictionary<string, IEnumerable<IProvince>>();
        private readonly IDatabaseUnitOfWorkProvider _uowProvider;
        private readonly RepositoryFactory _repositoryFactory;
        private readonly MerchelloConfiguration _configuration;

        public RegionService()
            : this(MerchelloConfiguration.Current)
        { }

        public RegionService(MerchelloConfiguration configuration)
            : this(new RepositoryFactory(), configuration)
        { }

        public RegionService(RepositoryFactory repositoryFactory, MerchelloConfiguration configuration)
            : this(new PetaPocoUnitOfWorkProvider(), repositoryFactory, configuration)
        { }

        public RegionService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, MerchelloConfiguration configuration)
        {
            Mandate.ParameterNotNull(configuration, "configuration");
            Mandate.ParameterNotNull(repositoryFactory, "repositoryFactory");
            Mandate.ParameterNotNull(provider, "provider");

            _repositoryFactory = repositoryFactory;
            _uowProvider = provider;
            _configuration = configuration;

            if (!RegionProvinceCache.IsEmpty) return;

            foreach (RegionElement region in configuration.Section.RegionalProvinces)
            {
                CacheRegion(region.Code, (from ProvinceElement pe in region.Provinces
                    select new Province(pe.Code, pe.Name)).Cast<IProvince>().ToArray());
            }
        }

        private static void CacheRegion(string code, IProvince[] provinces)
        {
            RegionProvinceCache.AddOrUpdate(code, provinces, (x, y) => provinces);
        }

        /// <summary>
        /// Returns the <see cref="Region" /> for the country code passed.
        /// </summary>
        /// <param name="regionCode">The two letter ISO Region code (country code)</param>
        /// <returns><see cref="RegionInfo"/> for the country corresponding the the country code passed</returns>
        public Region GetRegionByCode(string regionCode)
        {
            return new Region(regionCode, GetProvincesByCode(regionCode))
            {
                ProvinceLabel = GetProvinceLabelForRegion(regionCode)
            };
        }

        /// <summary>
        /// Returns a Region collection for all countries
        /// </summary>
        /// <returns>A collection of <see cref="RegionInfo"/></returns>
        public IEnumerable<Region> GetAllRegions()
        {
            return CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                .Select(culture => new RegionInfo(culture.Name))
                .Select(ri => 
                    new Region(ri.TwoLetterISORegionName, GetProvincesByCode(ri.TwoLetterISORegionName))
                    {
                        ProvinceLabel = GetProvinceLabelForRegion(ri.TwoLetterISORegionName)
                    }
                );
        }

        /// <summary>
        /// Returns a Region collection for all countries excluding codes passed
        /// </summary>
        /// <param name="excludeCodes">A collection of country codes to exclude from the result set</param>
        /// <returns>A collection of <see cref="RegionInfo"/></returns>
        public IEnumerable<Region> GetAllRegions(string[] excludeCodes)
        {
            return GetAllRegions().Where(x => !excludeCodes.Contains(x.RegionInfo.TwoLetterISORegionName));
        }


        /// <summary>
        /// True/false indicating whether or not the region has provinces configured in the Merchello.config file
        /// </summary>
        /// <param name="regionCode">The two letter ISO Region code (country code)</param>
        /// <returns></returns>
        private bool RegionHasProvinces(string regionCode)
        {
            return RegionProvinceCache.ContainsKey(regionCode);
        }

        /// <summary>
        /// Returns the province label from the configuration file
        /// </summary>
        /// <param name="regionCode">The two letter ISO Region code</param>
        /// <returns></returns>
        private string GetProvinceLabelForRegion(string regionCode)
        {
            return RegionHasProvinces(regionCode)
                ? _configuration.Section.RegionalProvinces[regionCode].ProvinceLabel
                : string.Empty;
        }

        /// <summary>
        /// Returns a collection of <see cref="IProvince"/> given a region code
        /// </summary>
        /// <param name="regionCode">The two letter ISO Region code (country code)</param>
        /// <returns>A collection of <see cref="IProvince"/></returns>
        private IEnumerable<IProvince> GetProvincesByCode(string regionCode)
        {
            return RegionHasProvinces(regionCode) ? 
                RegionProvinceCache[regionCode] : 
                new List<IProvince>();
        }
    }
}