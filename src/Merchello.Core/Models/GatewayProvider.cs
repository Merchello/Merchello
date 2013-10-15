using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using Merchello.Core.Models.TypeFields;

namespace Merchello.Core.Models
{
    internal class GatewayProvider : GatewayProviderBase, IGatewayProvider
    {
        private Guid _gatewayProviderTfKey;
        private string _name;
        private string _typeFullName;
        private string _configurationData;
        private bool _encryptConfigurationData;
        private Gateway.IGatewayProvider _gatewayProvider;


        public GatewayProvider()
            : this(null)
        { }

        public GatewayProvider(Gateway.IGatewayProvider gatewayProvider)
        {
            _gatewayProvider = gatewayProvider;
        }

        private static readonly PropertyInfo GatewayTypeFieldKeySelector = ExpressionHelper.GetPropertyInfo<GatewayProvider, Guid>(x => x.GatewayProviderTfKey);
        private static readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<GatewayProvider, string>(x => x.Name);
        private static readonly PropertyInfo TypeFullNameSelector = ExpressionHelper.GetPropertyInfo<GatewayProvider, string>(x => x.TypeFullName);
        private static readonly PropertyInfo ConfigurationDataSelector = ExpressionHelper.GetPropertyInfo<GatewayProvider, string>(x => x.ConfigurationData);
        private static readonly PropertyInfo EncryptConfigurationDatSelector = ExpressionHelper.GetPropertyInfo<GatewayProvider, bool>(x => x.EncryptConfigurationData);

        /// <summary>
        /// The GatewayProviderTypeFieldKey
        /// </summary>
        [DataMember]
        public Guid GatewayProviderTfKey
        {
            get { return _gatewayProviderTfKey; }
            set 
            { 
                SetPropertyValueAndDetectChanges(o =>
                {
                    _gatewayProviderTfKey = value;
                    return _gatewayProviderTfKey;
                }, _gatewayProviderTfKey, GatewayTypeFieldKeySelector); 
            }
        }

        /// <summary>
        /// The name of the gateway provider
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
        /// The Type.FullName of the gateway provider used to instantiate the provider
        /// </summary>
        [DataMember]
        public string TypeFullName
        {
            get { return _typeFullName; }
            set 
            { 
                SetPropertyValueAndDetectChanges(o =>
                {
                    _typeFullName = value;
                    return _typeFullName;
                }, _typeFullName, TypeFullNameSelector); 
            }
        }

        /// <summary>
        /// The configuration data associated with the gateway provider stored in xml format
        /// </summary>
        [DataMember]
        public string ConfigurationData
        {
            get { return _configurationData; }
            set 
            { 
                SetPropertyValueAndDetectChanges(o =>
                {
                    _configurationData = value;
                    return _configurationData;
                }, _configurationData, ConfigurationDataSelector); 
            }
        }

        /// <summary>
        /// True/false indicating whether or not the configuration data should be encrypted when persisted
        /// </summary>
        [DataMember]
        public bool EncryptConfigurationData
        {
            get { return _encryptConfigurationData; }
            set 
            { 
                SetPropertyValueAndDetectChanges(o =>
                {
                    _encryptConfigurationData = value;
                    return _configurationData;
                }, _configurationData, EncryptConfigurationDatSelector); 
            }
        }

        /// <summary>
        /// The gateway provider type
        /// </summary>
        [DataMember]
        public GatewayProviderType GatewayProviderType
        {
            get { return EnumTypeFieldConverter.GatewayProvider.GetTypeField(_gatewayProviderTfKey); }
            set
            {
                var reference = EnumTypeFieldConverter.GatewayProvider.GetTypeField(value);
                if (!ReferenceEquals(TypeFieldMapperBase.NotFound, reference))
                {
                    // call through the property to flag the dirty property
                    GatewayProviderTfKey = reference.TypeKey;
                }
            }
        }

        /// <summary>
        /// Creates an in
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="settings"></param>
        /// <returns></returns>
        public override Gateway.IGatewayProvider CreateInstance<T>(IDictionary<string, string> settings)
        {
            if (_gatewayProvider != null) return _gatewayProvider as T;
            return null;
        }
    }
}