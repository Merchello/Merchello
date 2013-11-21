using System.Collections.Generic;
using System.Globalization;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.Interfaces;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Represents a shipping or tax region (country)
    /// </summary>
    public class Region : Entity, IRegion
    {
        private readonly string _code;
        private readonly RegionInfo _regionInfo;
        private readonly IEnumerable<IProvince> _provinces; 

        public Region(string code)
            : this(code, new List<Province>())
        { }

        public Region(string code, IEnumerable<IProvince> provinces)
            : this(code, new RegionInfo(code), provinces)
        {}

        public Region(string code, RegionInfo regionInfo, IEnumerable<IProvince> provinces)
        {
            _code = code;
            _regionInfo = regionInfo;
            _provinces = provinces;
        }

        /// <summary>
        /// The two letter ISO Region code
        /// </summary>
        public string Code 
        {
            get { return _code; }
        }

        /// <summary>
        /// The <see cref="System.Globalization.RegionInfo"/> associated with the Region
        /// </summary>
        public RegionInfo RegionInfo { get { return _regionInfo; } }

        /// <summary>
        /// The label associated with the province list.  (eg. for US this would be 'States')
        /// </summary>
        public string ProvinceLabel { get; set; }

        /// <summary>
        /// The provinces (if any) associated with the region
        /// </summary>
        public IEnumerable<IProvince> Provinces {
            get { return _provinces; }
        }
    }
}