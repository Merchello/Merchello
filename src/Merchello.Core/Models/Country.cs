using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Implementation of Country
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class Country : CountryBase
    {
        public Country(string countryCode) : base(countryCode)
        {
        }

        public Country(string countryCode, IEnumerable<IProvince> provinces) : base(countryCode, provinces)
        {
        }

        public Country(string countryCode, RegionInfo regionInfo, IEnumerable<IProvince> provinces) : base(countryCode, regionInfo, provinces)
        {
        }
    }
}