using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Represents a shipping or tax region (country)
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public abstract class CountryBase : Entity, ICountryBase
    {
        private readonly string _countryCode;
        private readonly RegionInfo _regionInfo;
        private readonly IEnumerable<IProvince> _provinces;
        

        protected CountryBase(string countryCode, IEnumerable<IProvince> provinces)
            : this(countryCode, countryCode.Equals(Constants.CountryCodes.EverywhereElse) ? null : new RegionInfo(countryCode), provinces)
        { }

        protected CountryBase(string countryCode, RegionInfo regionInfo, IEnumerable<IProvince> provinces)
        {
            if(!countryCode.Equals(Constants.CountryCodes.EverywhereElse)) Mandate.ParameterNotNull(regionInfo, "regionInfo");
            Mandate.ParameterNotNull(provinces, "provinces");

            _countryCode = countryCode;
            if (!countryCode.Equals(Constants.CountryCodes.EverywhereElse))  _regionInfo = regionInfo;
            _provinces = provinces;
        }

        /// <summary>
        /// The two letter ISO Region code
        /// </summary>
        [DataMember]
        public string CountryCode 
        {
            get { return _countryCode; }
        }

        /// <summary>
        /// The English name associated with the region
        /// </summary>
        [DataMember]
        public string Name {
            get
            {
                return _countryCode.Equals(Constants.CountryCodes.EverywhereElse) ? "Everywhere Else" : _regionInfo.EnglishName;
            }
        }

        /// <summary>
        /// The <see cref="System.Globalization.RegionInfo"/> associated with the Region
        /// </summary>
        [DataMember]
        public RegionInfo RegionInfo { get { return _regionInfo; } }

        /// <summary>
        /// The label associated with the province list.  (eg. for US this would be 'States')
        /// </summary>
        [DataMember]
        public string ProvinceLabel { get; internal set; }

        /// <summary>
        /// Provinces (if any) associated with the country
        /// </summary>
        [IgnoreDataMember]
        public IEnumerable<IProvince> Provinces {
            get { return _provinces; }
        }
    }
}