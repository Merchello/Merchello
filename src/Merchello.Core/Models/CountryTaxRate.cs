using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Represents a province from a taxation context
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    internal class CountryTaxRate : Entity, ICountryTaxRate
    {
        private readonly Guid _providerKey;

        public CountryTaxRate(Guid providerKey, ICountry country)
        {
            Mandate.ParameterCondition(providerKey != Guid.Empty, "providerKey");
            Mandate.ParameterNotNull(country, "country");
            _providerKey = providerKey;
            PercentageTaxRate = 0;
        }

        [DataMember]
        public Guid ProviderKey {
            get { return _providerKey; }
        }

        /// <summary>
        /// Tax percentage rate for orders shipped to this province
        /// </summary>
        [DataMember]
        public decimal PercentageTaxRate { get; set; }

        [DataMember]
        public new ProvinceCollection<ITaxProvince> Provinces { get; set; }
    }
}