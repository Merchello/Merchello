using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models.Interfaces
{
    /// <summary>
    /// Represents a region
    /// </summary>
    public interface IRegion : IEntity  
    {
       
        /// <summary>
        /// The two letter ISO Region code
        /// </summary>
        string Code { get; }

        /// <summary>
        /// The <see cref="System.Globalization.RegionInfo"/> associated with the Region
        /// </summary>
        RegionInfo RegionInfo { get; } 

        /// <summary>
        /// The label associated with the province list.  (eg. for US this would be 'States')
        /// </summary>
        string ProvinceLabel { get; set; }

        /// <summary>
        /// The provinces (if any) associated with the region
        /// </summary>
        IEnumerable<IProvince> Provinces { get; }
    }
}