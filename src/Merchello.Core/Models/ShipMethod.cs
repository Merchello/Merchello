namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Specialized;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Models.Interfaces;

    using Umbraco.Core;

    /// <inheritdoc/>
    [Serializable]
    [DataContract(IsReference = true)]
    internal class ShipMethod : Entity, IShipMethod
    {
        /// <summary>
        /// The property selectors.
        /// </summary>
        private static readonly Lazy<PropertySelectors> _ps = new Lazy<PropertySelectors>();

        /// <summary>
        /// The name.
        /// </summary>
        private string _name;

        /// <summary>
        /// The provider key.
        /// </summary>
        private readonly Guid _providerKey;

        /// <summary>
        /// The ship country key.
        /// </summary>
        private readonly Guid _shipCountryKey;

        /// <summary>
        /// The surcharge.
        /// </summary>
        private decimal _surcharge;

        /// <summary>
        /// The service code.
        /// </summary>
        private string _serviceCode;

        /// <summary>
        /// The taxable.
        /// </summary>
        private bool _taxable;

        /// <summary>
        /// The ship provinces.
        /// </summary>
        private ProvinceCollection<IShipProvince> _shipProvinces;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShipMethod"/> class.
        /// </summary>
        /// <param name="providerKey">
        /// The provider key.
        /// </param>
        /// <param name="shipCountryKey">
        /// The ship country key.
        /// </param>
        internal ShipMethod(Guid providerKey, Guid shipCountryKey)
            : this(providerKey, shipCountryKey, new ProvinceCollection<IShipProvince>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShipMethod"/> class.
        /// </summary>
        /// <param name="providerKey">
        /// The provider key.
        /// </param>
        /// <param name="shipCountryKey">
        /// The ship country key.
        /// </param>
        /// <param name="shipProvinces">
        /// The ship provinces.
        /// </param>
        internal ShipMethod(Guid providerKey, Guid shipCountryKey, ProvinceCollection<IShipProvince> shipProvinces)
        {
            Ensure.ParameterCondition(Guid.Empty != providerKey, "providerKey");
            Ensure.ParameterCondition(Guid.Empty != shipCountryKey, "shipCountryKey");
            Ensure.ParameterNotNull(shipProvinces, "provinces");

            _providerKey = providerKey;
            _shipCountryKey = shipCountryKey;
            _shipProvinces = shipProvinces;
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
        public Guid ProviderKey
        {
            get
            {
                return _providerKey;
            }           
        }

        /// <inheritdoc/>
        [DataMember]
        public Guid ShipCountryKey
        {
            get
            {
                return _shipCountryKey;
            }           
        }

        /// <inheritdoc/>
        [DataMember]
        public decimal Surcharge
        {
            get
            {
                return _surcharge;
            }

            set 
            { 
                SetPropertyValueAndDetectChanges(value, ref _surcharge, _ps.Value.SurchargeSelector); 
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string ServiceCode
        {
            get
            {
                return _serviceCode;
            }

            set 
            { 
                SetPropertyValueAndDetectChanges(value, ref _serviceCode, _ps.Value.ServiceCodeSelector); 
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public bool Taxable
        {
            get
            {
                return _taxable;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _taxable, _ps.Value.TaxableSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public ProvinceCollection<IShipProvince> Provinces
        {
            get
            {
                return _shipProvinces;
            }

            set
            {
                _shipProvinces = value;
                _shipProvinces.CollectionChanged += ShipProvincesChanged;
            }
        }

        /// <summary>
        /// Handles the ship provinces changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ShipProvincesChanged(object sender, NotifyCollectionChangedEventArgs e)
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
            public readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<ShipMethod, string>(x => x.Name);

            /// <summary>
            /// The surcharge selector.
            /// </summary>
            public readonly PropertyInfo SurchargeSelector = ExpressionHelper.GetPropertyInfo<ShipMethod, decimal>(x => x.Surcharge);

            /// <summary>
            /// The service code selector.
            /// </summary>
            public readonly PropertyInfo ServiceCodeSelector = ExpressionHelper.GetPropertyInfo<ShipMethod, string>(x => x.ServiceCode);

            /// <summary>
            /// The taxable selector.
            /// </summary>
            public readonly PropertyInfo TaxableSelector = ExpressionHelper.GetPropertyInfo<ShipMethod, bool>(x => x.Taxable);

            /// <summary>
            /// The provinces changed selector.
            /// </summary>
            public readonly PropertyInfo ProvincesChangedSelector = ExpressionHelper.GetPropertyInfo<ShipMethod, ProvinceCollection<IShipProvince>>(x => x.Provinces);
        }
    }
}