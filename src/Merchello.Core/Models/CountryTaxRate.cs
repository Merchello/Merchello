using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.Interfaces;

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
        private readonly string _code;
        private decimal _percentageTaxRate;
        private ProvinceCollection<ITaxProvince> _taxProvinces;

        internal CountryTaxRate(Guid providerKey, string code)
        {
            Mandate.ParameterCondition(providerKey != Guid.Empty, "providerKey");
            Mandate.ParameterNotNullOrEmpty(code, "countryCode");

            _providerKey = providerKey;
            _code = code;
            PercentageTaxRate = 0;
        }

        private static readonly PropertyInfo PercentageTaxRateSelector = ExpressionHelper.GetPropertyInfo<CountryTaxRate, decimal>(x => x.PercentageTaxRate);
        private static readonly PropertyInfo ProvincesChangedSelector = ExpressionHelper.GetPropertyInfo<CountryTaxRate, ProvinceCollection<ITaxProvince>>(x => x.Provinces);

        private void TaxProvincesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(ProvincesChangedSelector);
        }
     
        /// <summary>
        /// The key associated with the gateway provider for the tax rate data
        /// </summary>
        [DataMember]
        public Guid ProviderKey {
            get { return _providerKey; }
        }

        /// <summary>
        /// The two digit ISO Country code
        /// </summary>
        [DataMember]
        public string Code {
            get { return _code; }
        }

        /// <summary>
        /// Tax percentage rate for orders shipped to this province
        /// </summary>
        [DataMember]
        public decimal PercentageTaxRate
        {
            get { return _percentageTaxRate; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _percentageTaxRate = value;
                    return _percentageTaxRate;
                }, _percentageTaxRate, PercentageTaxRateSelector);
            }
        }

        /// <summary>
        /// Stores province adjustments (if any) for the tax country
        /// </summary>
        [DataMember]
        public ProvinceCollection<ITaxProvince> Provinces
        {
            get { return _taxProvinces; }
            set
            {
                _taxProvinces = value;
                _taxProvinces.CollectionChanged += TaxProvincesChanged;
            }
        }
    }
}