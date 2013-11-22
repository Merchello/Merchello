using System.Collections.Generic;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.Interfaces;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a Country
    /// </summary>
    public interface ICountry : IEntity
    {
        /// <summary>
        /// The two letter ISO Region code
        /// </summary>
        [DataMember]
        string CountryCode { get; }

        /// <summary>
        /// The English name associated with the region
        /// </summary>
        [DataMember]
        string Name { get; }

        /// <summary>
        /// The label associated with the province list.  (eg. for US this would be 'States')
        /// </summary>
        string ProvinceLabel { get; }

        /// <summary>
        /// Provinces (if any) associated with the country
        /// </summary>
        [DataMember]
        IEnumerable<IProvince> Provinces { get; }
    }
}