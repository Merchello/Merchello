namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;

    using Umbraco.Core;

    using Constants = Merchello.Core.Constants;

    /// <summary>
    /// Represents a shipping or tax region (country)
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public abstract class CountryBase : Entity, ICountryBase
    {
        /// <summary>
        /// The country code.
        /// </summary>
        private readonly string _countryCode;

        /// <summary>
        /// The <see cref="RegionInfo"/>.
        /// </summary>
        private readonly RegionInfo _regionInfo;

        /// <summary>
        /// The provinces.
        /// </summary>
        private readonly IEnumerable<IProvince> _provinces;

        /// <summary>
        /// Initializes a new instance of the <see cref="CountryBase"/> class.
        /// </summary>
        /// <param name="countryCode">
        /// The country code.
        /// </param>
        /// <param name="provinces">
        /// The provinces.
        /// </param>
        protected CountryBase(string countryCode, IEnumerable<IProvince> provinces)
            : this(
                countryCode,
                countryCode.Equals(Constants.CountryCodes.EverywhereElse) ? null : new RegionInfo(countryCode),
                provinces)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CountryBase"/> class.
        /// </summary>
        /// <param name="countryCode">
        /// The country code.
        /// </param>
        /// <param name="regionInfo">
        /// The region info.
        /// </param>
        /// <param name="provinces">
        /// The provinces.
        /// </param>
        protected CountryBase(string countryCode, RegionInfo regionInfo, IEnumerable<IProvince> provinces)
        {   
            var proviceArray = provinces as IProvince[] ?? provinces.ToArray();

            Mandate.ParameterNotNull(proviceArray, "provinces");

            _countryCode = countryCode;
            if (!countryCode.Equals(Constants.CountryCodes.EverywhereElse))  _regionInfo = regionInfo;
            _provinces = proviceArray;
        }

        /// <summary>
        /// Gets the two letter ISO Region code
        /// </summary>
        [DataMember]
        public string CountryCode 
        {
            get { return _countryCode; }
        }

        /// <summary>
        /// Gets the English name associated with the region
        /// </summary>
        [DataMember]
        public string Name 
        {
            get
            {
                return _countryCode.Equals(Constants.CountryCodes.EverywhereElse) ? "Everywhere Else" : _regionInfo.EnglishName;
            }
        }

        /// <summary>
        /// Gets the <see cref="System.Globalization.RegionInfo"/> associated with the Region
        /// </summary>
        [DataMember]
        public RegionInfo RegionInfo
        {
            get
            {
                return _regionInfo;
            }
        }

        /// <summary>
        /// Gets the label associated with the province list.  (ex. for US this would be 'States')
        /// </summary>
        [DataMember]
        public string ProvinceLabel { get; internal set; }

        /// <summary>
        /// Gets the Provinces (if any) associated with the country
        /// </summary>
        [IgnoreDataMember]
        public IEnumerable<IProvince> Provinces 
        {
            get { return _provinces; }
        }
    }
}