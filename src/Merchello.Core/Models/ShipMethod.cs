using System;
using System.Collections.Specialized;
using System.Reflection;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.Interfaces;

namespace Merchello.Core.Models
{
    using Umbraco.Core;

    [Serializable]
    [DataContract(IsReference = true)]
    internal class ShipMethod : Entity, IShipMethod
    {
        private string _name;
        private readonly Guid _providerKey;
        private readonly Guid _shipCountryKey;
        private decimal _surcharge;
        private string _serviceCode;
        private bool _taxable;
        private ProvinceCollection<IShipProvince> _shipProvinces;

        internal ShipMethod(Guid providerKey, Guid shipCountryKey)
            : this(providerKey,shipCountryKey, new ProvinceCollection<IShipProvince>())
        {}

        internal ShipMethod(Guid providerKey, Guid shipCountryKey, ProvinceCollection<IShipProvince> shipProvinces)
        {
            Mandate.ParameterCondition(Guid.Empty != providerKey, "providerKey");
            Mandate.ParameterCondition(Guid.Empty != shipCountryKey, "shipCountryKey");
            Mandate.ParameterNotNull(shipProvinces, "provinces");

            _providerKey = providerKey;
            _shipCountryKey = shipCountryKey;
            _shipProvinces = shipProvinces;
        }

        private static readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<ShipMethod, string>(x => x.Name); 
        private static readonly PropertyInfo SurchargeSelector = ExpressionHelper.GetPropertyInfo<ShipMethod, decimal>(x => x.Surcharge);  
        private static readonly PropertyInfo ServiceCodeSelector = ExpressionHelper.GetPropertyInfo<ShipMethod, string>(x => x.ServiceCode);
        private static readonly PropertyInfo TaxableSelector = ExpressionHelper.GetPropertyInfo<ShipMethod, bool>(x => x.Taxable);
        private static readonly PropertyInfo ProvincesChangedSelector = ExpressionHelper.GetPropertyInfo<ShipMethod, ProvinceCollection<IShipProvince>>(x => x.Provinces);

        private void ShipProvincesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(ProvincesChangedSelector);
        }
        
        /// <summary>
        /// The name associated with the ShipMethod
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
        /// The provider key associated with the ship method
        /// </summary>
        [DataMember]
        public Guid ProviderKey
        {
            get { return _providerKey; }           
        }

        /// <summary>
        /// The key associated with the ship country for the Ship Method
        /// </summary>
        public Guid ShipCountryKey
        {
            get { return _shipCountryKey; }           
        }

        /// <summary>
        /// The surcharge associated with the ShipMethod
        /// </summary>
        [DataMember]
        public decimal Surcharge
        {
            get { return _surcharge; }
            set 
            { 
                SetPropertyValueAndDetectChanges(o =>
                {
                    _surcharge = value;
                    return _surcharge;
                }, _surcharge, SurchargeSelector); 
            }
        }
    
        /// <summary>
        /// The serviceCode associated with the ShipMethod
        /// </summary>
        [DataMember]
        public string ServiceCode
        {
            get { return _serviceCode; }
            set 
            { 
                SetPropertyValueAndDetectChanges(o =>
                {
                    _serviceCode = value;
                    return _serviceCode;
                }, _serviceCode, ServiceCodeSelector); 
            }
        }

        /// <summary>
        /// True/false indicating whether or not this shipmethod is taxable
        /// </summary>
        [DataMember]
        public bool Taxable
        {
            get { return _taxable; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _taxable = value;
                    return _taxable;
                }, _taxable, TaxableSelector);
            }
        }
        /// <summary>
        /// A collection of provinces 
        /// </summary>
        [DataMember]
        public ProvinceCollection<IShipProvince> Provinces
        {
            get { return _shipProvinces; }
            set
            {
                _shipProvinces = value;
                _shipProvinces.CollectionChanged += ShipProvincesChanged;
            }
        }

                    
    }
}