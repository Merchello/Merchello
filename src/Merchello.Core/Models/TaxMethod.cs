namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Represents a province from a taxation context
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    internal class TaxMethod : Entity, ITaxMethod
    {
        /// <summary>
        /// The property selectors.
        /// </summary>
        private static readonly Lazy<PropertySelectors> _ps = new Lazy<PropertySelectors>();

        #region Fields

        /// <summary>
        /// The provider key.
        /// </summary>
        private readonly Guid _providerKey;

        /// <summary>
        /// The country code.
        /// </summary>
        private readonly string _countryCode;

        /// <summary>
        /// The name.
        /// </summary>
        private string _name;

        /// <summary>
        /// The percentage tax rate.
        /// </summary>
        private decimal _percentageTaxRate;

        /// <summary>
        /// The product tax method.
        /// </summary>
        private bool _productTaxMethod;

        /// <summary>
        /// The tax provinces.
        /// </summary>
        private ProvinceCollection<ITaxProvince> _taxProvinces;

        #endregion

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
            Ensure.ParameterCondition(providerKey != Guid.Empty, "providerKey");
            Ensure.ParameterNotNullOrEmpty(countryCode, "countryCode");

            _providerKey = providerKey;
            _countryCode = countryCode;
            PercentageTaxRate = 0;
        }
     
        /// <inheritdoc/>
        [DataMember]
        public Guid ProviderKey
        {
            get
            {
                return _providerKey;
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _name, _ps.Value.NameSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string CountryCode
        {
            get
            {
                return _countryCode;
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public decimal PercentageTaxRate
        {
            get
            {
                return _percentageTaxRate;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _percentageTaxRate, _ps.Value.PercentageTaxRateSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public ProvinceCollection<ITaxProvince> Provinces
        {
            get
            {
                return _taxProvinces;
            }

            set
            {
                _taxProvinces = value;
                _taxProvinces.CollectionChanged += TaxProvincesChanged;
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public bool ProductTaxMethod
        {
            get
            {
                return _productTaxMethod;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _productTaxMethod, _ps.Value.ProductTaxMethodSelector);
            }
        }

        /// <inheritdoc/>
        [IgnoreDataMember]
        public bool HasProvinces
        {
            get
            {
                return _taxProvinces.Any();
            }
        }

        /// <summary>
        /// Handles the tax provinces changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void TaxProvincesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(_ps.Value.ProvincesChangedSelector);
        }

        /// <summary>
        /// The property selectors.
        /// </summary>
        private class PropertySelectors
        {
            /// <summary>
            /// The name selector.
            /// </summary>
            public readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<TaxMethod, string>(x => x.Name);

            /// <summary>
            /// The percentage tax rate selector.
            /// </summary>
            public readonly PropertyInfo PercentageTaxRateSelector = ExpressionHelper.GetPropertyInfo<TaxMethod, decimal>(x => x.PercentageTaxRate);

            /// <summary>
            /// The provinces changed selector.
            /// </summary>
            public readonly PropertyInfo ProvincesChangedSelector = ExpressionHelper.GetPropertyInfo<TaxMethod, ProvinceCollection<ITaxProvince>>(x => x.Provinces);

            /// <summary>
            /// The product tax method selector.
            /// </summary>
            public readonly PropertyInfo ProductTaxMethodSelector = ExpressionHelper.GetPropertyInfo<TaxMethod, bool>(x => x.ProductTaxMethod);
        }
    }
}