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
    internal class Country : CountryBase
    {
        internal Country(string countryCode) 
            : this(countryCode, new List<IProvince>())
        { }

        internal Country(string countryCode, IEnumerable<IProvince> provinces) 
            : base(countryCode, provinces)
        { }

        internal Country(string countryCode, RegionInfo regionInfo, IEnumerable<IProvince> provinces) 
            : base(countryCode, regionInfo, provinces)
        { }
    }
}