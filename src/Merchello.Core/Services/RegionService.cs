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
    public class RegionService : IRegionService
    {
        private readonly static ConcurrentDictionary<string, IEnumerable<IProvince>> RegionProvinceCache = new ConcurrentDictionary<string, IEnumerable<IProvince>>();
        private readonly IDatabaseUnitOfWorkProvider _uowProvider;
        private readonly RepositoryFactory _repositoryFactory;

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

            BuildCache(configuration);
        }

        #region Initialization


        private static void BuildCache(MerchelloConfiguration configuration)
        {
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

        #endregion

        /// <summary>
        /// Returns the <see cref="RegionInfo"/> for the country code passed.
        /// </summary>
        /// <param name="regionCode">The two letter ISO Region code (country code)</param>
        /// <returns><see cref="RegionInfo"/> for the country corresponding the the country code passed</returns>
        public RegionInfo GetRegionInfoByCode(string regionCode)
        {
            return new RegionInfo(regionCode);
        }

        /// <summary>
        /// Returns a RegionInfo collection for all countries
        /// </summary>
        /// <returns>A collection of <see cref="RegionInfo"/></returns>
        public IEnumerable<RegionInfo> GetRegionInfoList()
        {
            return CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                              .Select(culture => new RegionInfo(culture.Name));
        }

        /// <summary>
        /// Returns a RegionInfo collection for all countries excluding codes passed
        /// </summary>
        /// <param name="excludeCodes">A collection of country codes to exclude from the result set</param>
        /// <returns>A collection of <see cref="RegionInfo"/></returns>
        public IEnumerable<RegionInfo> GetRegionInfoList(string[] excludeCodes)
        {
            return GetRegionInfoList().Where(x => !excludeCodes.Contains(x.TwoLetterISORegionName));
        }

        /// <summary>
        /// True/false indicating whether or not the region has provinces configured in the Merchello.config file
        /// </summary>
        /// <param name="regionCode">The two letter ISO Region code (country code)</param>
        /// <returns></returns>
        public bool RegionHasProvinces(string regionCode)
        {
            return RegionProvinceCache.ContainsKey(regionCode);
        }

        /// <summary>
        /// Returns a collection of <see cref="IProvince"/> given a region code
        /// </summary>
        /// <param name="regionCode">The two letter ISO Region code (country code)</param>
        /// <returns>A collection of <see cref="IProvince"/></returns>
        public IEnumerable<IProvince> GetProvincesByCode(string regionCode)
        {
            return RegionHasProvinces(regionCode) ? 
                RegionProvinceCache[regionCode] : 
                new List<IProvince>();
        }
    }
}