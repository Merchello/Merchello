using System;
using System.Collections.Specialized;
using System.Reflection;
using System.Runtime.Serialization;
using Merchello.Core.Configuration.Outline;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.Interfaces;
using Merchello.Core.Models.TypeFields;

namespace Merchello.Core.Models
{

    [Serializable]
    [DataContract(IsReference = true)]
    internal class ShipMethod : Entity, IShipMethod
    {
        private string _name;
        private Guid _warehouseCountryKey;
        private Guid _providerKey;
        private Guid _shipMethodTfKey;
        private decimal _surcharge;
        private string _serviceCode;
        private ProvinceCollection<IShipProvince> _provinces;

        private static readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<ShipMethod, string>(x => x.Name);
        private static readonly PropertyInfo WarehouseCountryKeySelector = ExpressionHelper.GetPropertyInfo<ShipMethod, Guid>(x => x.WarehouseCountryKey);  
        private static readonly PropertyInfo ProviderKeySelector = ExpressionHelper.GetPropertyInfo<ShipMethod, Guid>(x => x.ProviderKey);  
        private static readonly PropertyInfo ShipMethodTypeFieldKeySelector = ExpressionHelper.GetPropertyInfo<ShipMethod, Guid>(x => x.ShipMethodTfKey);  
        private static readonly PropertyInfo SurchargeSelector = ExpressionHelper.GetPropertyInfo<ShipMethod, decimal>(x => x.Surcharge);  
        private static readonly PropertyInfo ServiceCodeSelector = ExpressionHelper.GetPropertyInfo<ShipMethod, string>(x => x.ServiceCode);
        private static readonly PropertyInfo ProvincesChangedSelector = ExpressionHelper.GetPropertyInfo<ShipMethod, ProvinceCollection<IShipProvince>>(x => x.Provinces);

        private void ProvincesChanged(object sender, NotifyCollectionChangedEventArgs e)
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
        /// The key (guid) associated with the Warehouse Country that offers this ship method
        /// </summary>
        [DataMember]
        public Guid WarehouseCountryKey
        {
            get { return _warehouseCountryKey; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _warehouseCountryKey = value;
                    return _warehouseCountryKey;
                }, _warehouseCountryKey, WarehouseCountryKeySelector);
            }
        }

        /// <summary>
        /// The provider key associated with the ship method
        /// </summary>
        [DataMember]
        public Guid ProviderKey
        {
            get { return _providerKey; }
            set 
            { 
                SetPropertyValueAndDetectChanges(o =>
                {
                    _providerKey = value;
                    return _providerKey;
                }, _providerKey, ProviderKeySelector); 
            }
        }
    
        /// <summary>
        /// The shipMethodTypeFieldKey associated with the ShipMethod
        /// </summary>
        [DataMember]
        public Guid ShipMethodTfKey
        {
            get { return _shipMethodTfKey; }
            set 
            { 
                SetPropertyValueAndDetectChanges(o =>
                {
                    _shipMethodTfKey = value;
                    return _shipMethodTfKey;
                }, _shipMethodTfKey, ShipMethodTypeFieldKeySelector); 
            }
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
        /// A collection of provinces 
        /// </summary>
        [DataMember]
        public ProvinceCollection<IShipProvince> Provinces
        {
            get { return _provinces; }
            set
            {
                _provinces = value;
                _provinces.CollectionChanged += ProvincesChanged;
            }
        }


        /// <summary>
        /// The ship method type 
        /// </summary>
        [DataMember]
        public ShipMethodType ShipMethodType
        {
            get { return EnumTypeFieldConverter.ShipmentMethod.GetTypeField(_shipMethodTfKey); }
            set
            {
                var reference = EnumTypeFieldConverter.ShipmentMethod.GetTypeField(value);
                if (!ReferenceEquals(TypeFieldMapperBase.NotFound, reference))
                {
                    // call through the property to flag the dirty property
                    ShipMethodTfKey = reference.TypeKey;
                }
            }
        }
                    
    }
}