namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;

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
        /// The English name of the country.
        /// </summary>
        private readonly string _name;

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
        [Obsolete]
        //// TODO country is internal so this can be removed - but wait for ShipCountry to be ported to 
        //// to work out all the kinks at the same time
        protected CountryBase(string countryCode, IEnumerable<IProvince> provinces)
            : this(
                countryCode,
                countryCode.Equals(Constants.CountryCodes.EverywhereElse) ? null : new RegionInfo(countryCode).EnglishName,
                provinces)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CountryBase"/> class.
        /// </summary>
        /// <param name="countryCode">
        /// The country code.
        /// </param>
        /// <param name="name">
        /// The name of the country.
        /// </param>
        /// <param name="provinces">
        /// The provinces.
        /// </param>
        protected CountryBase(string countryCode, string name, IEnumerable<IProvince> provinces)
        {   
            var proviceArray = provinces as IProvince[] ?? provinces.ToArray();

            Ensure.ParameterNotNull(proviceArray, "provinces");

            _countryCode = countryCode;
            if (!countryCode.Equals(Constants.CountryCodes.EverywhereElse)) _name = name;
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
                return _countryCode.Equals(Constants.CountryCodes.EverywhereElse) ? "Everywhere Else" : _name;
            }
        }

        /// <summary>
        /// Gets the English name associated with the region
        /// </summary>
        [DataMember]
        public int Iso { get; set; }


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