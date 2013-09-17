using System;
using System.Reflection;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.TypeFields;

namespace Merchello.Core.Models
{

    [Serializable]
    [DataContract(IsReference = true)]
    public class ShipMethod : IdEntity, IShipMethod
    {
        private string _name;
        private int _gatewayAlias;
        private Guid _shipMethodTypeFieldKey;
        private decimal _surcharge;
        private string _serviceCode;

        private static readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<ShipMethod, string>(x => x.Name);  
        private static readonly PropertyInfo GatewayAliasSelector = ExpressionHelper.GetPropertyInfo<ShipMethod, int>(x => x.GatewayAlias);  
        private static readonly PropertyInfo ShipMethodTypeFieldKeySelector = ExpressionHelper.GetPropertyInfo<ShipMethod, Guid>(x => x.ShipMethodTypeFieldKey);  
        private static readonly PropertyInfo SurchargeSelector = ExpressionHelper.GetPropertyInfo<ShipMethod, decimal>(x => x.Surcharge);  
        private static readonly PropertyInfo ServiceCodeSelector = ExpressionHelper.GetPropertyInfo<ShipMethod, string>(x => x.ServiceCode);  
        
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
        /// The gatewayAlias associated with the ShipMethod
        /// </summary>
        [DataMember]
        public int GatewayAlias
        {
            get { return _gatewayAlias; }
                set 
                { 
                    SetPropertyValueAndDetectChanges(o =>
                    {
                        _gatewayAlias = value;
                        return _gatewayAlias;
                    }, _gatewayAlias, GatewayAliasSelector); 
                }
        }
    
        /// <summary>
        /// The shipMethodTypeFieldKey associated with the ShipMethod
        /// </summary>
        [DataMember]
        public Guid ShipMethodTypeFieldKey
        {
            get { return _shipMethodTypeFieldKey; }
                set 
                { 
                    SetPropertyValueAndDetectChanges(o =>
                    {
                        _shipMethodTypeFieldKey = value;
                        return _shipMethodTypeFieldKey;
                    }, _shipMethodTypeFieldKey, ShipMethodTypeFieldKeySelector); 
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
        /// The ship method type 
        /// </summary>
        [DataMember]
        public ShipMethodType ShipMethodType
        {
            get { return EnumeratedTypeFieldConverter.ShipmentMethod().GetTypeField(_shipMethodTypeFieldKey); }
            set
            {
                var reference = EnumeratedTypeFieldConverter.ShipmentMethod().GetTypeField(value);
                if (!ReferenceEquals(TypeFieldMapperBase.NotFound, reference))
                {
                    // call through the property to flag the dirty property
                    ShipMethodTypeFieldKey = reference.TypeKey;
                }
            }
        }
                    
    }

}