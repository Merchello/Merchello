using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using Merchello.Core;
using Merchello.Core.Configuration;
using Merchello.Core.Configuration.Outline;
using NUnit.Framework;

namespace Merchello.Tests.UnitTests.Services
{
    [TestFixture]
    public class RegionInfoServiceTests
    {
        private IRegionService _regionService;

        [SetUp]
        public void Init()
        {
            _regionService = new RegionService();
        }
        
        [Test]
        public void Countries()
        {
            foreach (var regionInfo in _regionService.GetRegionInfoList(new [] { "SA", "DK" }))
            {
                Console.WriteLine("{0} {1} {2}", regionInfo.EnglishName, regionInfo.TwoLetterISORegionName.ToUpper(), regionInfo.CurrencySymbol);
            }
        }

        [Test]
        public void UsRegion()
        {
            var regionInfo = _regionService.GetRegionInfoByCode("CA");
            Console.Write("{0} {1} {2}", regionInfo.EnglishName, regionInfo.TwoLetterISORegionName.ToUpper(), regionInfo.CurrencySymbol);
        }
        
    }

   public class RegionService : IRegionService
   {
       private readonly static ConcurrentDictionary<string, IEnumerable<IProvince>> RegionProvinceCache = new ConcurrentDictionary<string, IEnumerable<IProvince>>();

       public RegionService()
           : this(MerchelloConfiguration.Current)
       { }

       public RegionService(MerchelloConfiguration configuration)
       {
           Mandate.ParameterNotNull(configuration, "configuration");

           BuildCache(configuration);   
       }


       private static void BuildCache(MerchelloConfiguration configuration)
       {
           foreach (RegionElement region in configuration.Section.RegionalProvinces)
           {
               CacheRegion(region.Code, (from ProvinceElement pe in region.Provinces
                                         select new Province()
                                             {
                                                 Code = pe.Code, Name = pe.Name
                                             }).Cast<IProvince>().ToArray());
           }
       }

       private static void CacheRegion(string code, IProvince[] provinces)
       {
           RegionProvinceCache.AddOrUpdate(code, provinces, (x, y) => provinces);
       }

       public RegionInfo GetRegionInfoByCode(string code)
       {
           return new RegionInfo(code);
       }

       public IEnumerable<RegionInfo> GetRegionInfoList()
       {
           return CultureInfo.GetCultures(CultureTypes.SpecificCultures)
               .Select(culture => new RegionInfo(culture.Name));
       }

       public IEnumerable<RegionInfo> GetRegionInfoList(string[] excludeCodes)
       {
           return GetRegionInfoList().Where(x => !excludeCodes.Contains(x.TwoLetterISORegionName));
       }

   }


    [Serializable]
    [DataContract(IsReference=true)]
    internal class Province : IProvince
    {
        [DataMember]
        public string Name { get; set; }
        
        [DataMember]
        public string Code { get; set; }
    }

    internal interface IProvince
    {
        [DataMember]
        string Name { get; set; }
        
        [DataMember]
        string Code { get; set; }
    }

    public interface IRegionService
    {
        RegionInfo GetRegionInfoByCode(string code);
        IEnumerable<RegionInfo> GetRegionInfoList();
        IEnumerable<RegionInfo> GetRegionInfoList(string[] strings);
    }
}