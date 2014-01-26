using System;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    public interface ICountryTaxRate : IEntity
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
 
    }
}