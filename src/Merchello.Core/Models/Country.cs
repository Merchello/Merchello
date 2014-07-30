namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.Serialization;

    /// <summary>
    /// Implementation of Country
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    internal class Country : CountryBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Country"/> class.
        /// </summary>
        /// <param name="countryCode">
        /// The country code.
        /// </param>
        internal Country(string countryCode)
            : this(countryCode, new List<IProvince>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Country"/> class.
        /// </summary>
        /// <param name="countryCode">
        /// The country code.
        /// </param>
        /// <param name="provinces">
        /// The provinces.
        /// </param>
        internal Country(string countryCode, IEnumerable<IProvince> provinces)
            : base(countryCode, provinces)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Country"/> class.
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
        internal Country(string countryCode, RegionInfo regionInfo, IEnumerable<IProvince> provinces)
            : base(countryCode, regionInfo, provinces)
        {
        }
    }
}