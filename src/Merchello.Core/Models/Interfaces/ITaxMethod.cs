using System;
using System.Globalization;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    public interface ITaxMethod : IEntity
    {
        /// <summary>
        /// The key associated with the gateway provider for the tax rate data
        /// </summary>
        [DataMember]
        Guid ProviderKey { get; }

        /// <summary>
        /// The two digit ISO Country code
        /// </summary>
        [DataMember]
        string CountryCode { get; }

        /// <summary>
        /// Percentage tax rate
        /// </summary>
        [DataMember]
        decimal PercentageTaxRate { get; set; }

        /// <summary>
        /// Stores province adjustments (if any) for the tax country
        /// </summary>
        [DataMember]
        ProvinceCollection<ITaxProvince> Provinces { get; set; }

        /// <summary>
        /// The <see cref="RegionInfo"/> for the country
        /// </summary>
        [IgnoreDataMember]
        RegionInfo RegionInfo { get; }

        /// <summary>
        /// True/false indicating whether or not the CountryTaxRate has provinces
        /// </summary>
        bool HasProvinces { get; }
    }
}