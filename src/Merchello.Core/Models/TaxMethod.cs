namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;

    using Umbraco.Core;

    /// <summary>
    /// Represents a province from a taxation context
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    internal class TaxMethod : Entity, ITaxMethod
    {

        /// <summary>
        /// The name selector.
        /// </summary>
        private static readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<TaxMethod, string>(x => x.Name);

        /// <summary>
        /// The percentage tax rate selector.
        /// </summary>
        private static readonly PropertyInfo PercentageTaxRateSelector = ExpressionHelper.GetPropertyInfo<TaxMethod, decimal>(x => x.PercentageTaxRate);

        /// <summary>
        /// The provinces changed selector.
        /// </summary>
        private static readonly PropertyInfo ProvincesChangedSelector = ExpressionHelper.GetPropertyInfo<TaxMethod, ProvinceCollection<ITaxProvince>>(x => x.Provinces);

        /// <summary>
        /// The product tax method selector.
        /// </summary>
        private static readonly PropertyInfo ProductTaxMethodSelector = ExpressionHelper.GetPropertyInfo<TaxMethod, bool>(x => x.ProductTaxMethod);

        /// <summary>
        /// The _provider key.
        /// </summary>
        private readonly Guid _providerKey;

        /// <summary>
        /// The _country code.
        /// </summary>
        private readonly string _countryCode;

        /// <summary>
        /// The _name.
        /// </summary>
        private string _name;

        /// <summary>
        /// The _percentage tax rate.
        /// </summary>
        private decimal _percentageTaxRate;

        /// <summary>
        /// The product tax method.
        /// </summary>
        private bool _productTaxMethod;

        /// <summary>
        /// The _tax provinces.
        /// </summary>
        private ProvinceCollection<ITaxProvince> _taxProvinces;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaxMethod"/> class.
        /// </summary>
        /// <param name="providerKey">
        /// The provider key.
        /// </param>
        /// <param name="countryCode">
        /// The country code.
        /// </param>
        internal TaxMethod(Guid providerKey, string countryCode)
        {
            Mandate.ParameterCondition(providerKey != Guid.Empty, "providerKey");
            Mandate.ParameterNotNullOrEmpty(countryCode, "countryCode");

            _providerKey = providerKey;
            _countryCode = countryCode;
            PercentageTaxRate = 0;
        }

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
        /// The name assoicated with the tax method
        /// </summary>
        [DataMember]
        public string Name
        {
            get { return _name; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _name = value;
                    return _name;
                }, _name, NameSelector);
            }
        }

        /// <summary>
        /// The two digit ISO Country code
        /// </summary>
        [DataMember]
        public string CountryCode {
            get { return _countryCode; }
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

        /// <summary>
        /// Gets or sets a value indicating whether product tax method.
        /// </summary>
        [DataMember]
        public bool ProductTaxMethod
        {
            get
            {
                return _productTaxMethod;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _productTaxMethod = value;
                    return _productTaxMethod;
                },
                _productTaxMethod, 
                ProductTaxMethodSelector);
            }
        }

        /// <summary>
        /// True/false indicating whether or not the CountryTaxRate has provinces
        /// </summary>
        [IgnoreDataMember]
        public bool HasProvinces {
            get { return _taxProvinces.Any(); }
        }
    }
}