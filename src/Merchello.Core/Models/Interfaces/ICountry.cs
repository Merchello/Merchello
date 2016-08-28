namespace Merchello.Core.Models
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Defines a Country
    /// </summary>
    public interface ICountry : IEntity
    {
        /// <summary>
        /// Gets the two letter ISO Region code
        /// </summary>
        [DataMember]
        string CountryCode { get; }

        /// <summary>
        /// Gets the English name associated with the region
        /// </summary>
        [DataMember]
        string Name { get; }

        /// <summary>
        /// Gets the label associated with the province list.  (e.g. for US this would be 'States')
        /// </summary>
        string ProvinceLabel { get; }

        /// <summary>
        /// Gets the collection of Provinces (if any) associated with the country
        /// </summary>
        [DataMember]
        IEnumerable<IProvince> Provinces { get; }
    }
}