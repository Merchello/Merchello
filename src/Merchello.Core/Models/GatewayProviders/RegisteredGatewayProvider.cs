using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Web;
using Merchello.Core.Gateway;
using Merchello.Core.Models.TypeFields;

namespace Merchello.Core.Models.GatewayProviders
{
    internal class RegisteredGatewayProvider : RegisteredGatewayProviderBase, IRegisteredGatewayProvider
    {
        private Guid _gatewayProviderTypeFieldKey;
        private string _name;
        private string _typeFullName;
        private string _configurationData;
        private bool _encryptConfigurationData;
        private IGatewayProvider _gatewayProvider;


        public RegisteredGatewayProvider()
            : this(null)
        { }

        public RegisteredGatewayProvider(IGatewayProvider gatewayProvider)
        {
            _gatewayProvider = gatewayProvider;
        }

        private static readonly PropertyInfo GatewayTypeFieldKeySelector = ExpressionHelper.GetPropertyInfo<RegisteredGatewayProvider, Guid>(x => x.GatewayProviderTypeFieldKey);
        private static readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<RegisteredGatewayProvider, string>(x => x.Name);
        private static readonly PropertyInfo TypeFullNameSelector = ExpressionHelper.GetPropertyInfo<RegisteredGatewayProvider, string>(x => x.TypeFullName);
        private static readonly PropertyInfo ConfigurationDataSelector = ExpressionHelper.GetPropertyInfo<RegisteredGatewayProvider, string>(x => x.ConfigurationData);
        private static readonly PropertyInfo EncryptConfigurationDatSelector = ExpressionHelper.GetPropertyInfo<RegisteredGatewayProvider, bool>(x => x.EncryptConfigurationData);

        /// <summary>
        /// The GatewayProviderTypeFieldKey
        /// </summary>
        [DataMember]
        public Guid GatewayProviderTypeFieldKey
        {
            get { return _gatewayProviderTypeFieldKey; }
            set 
            { 
                SetPropertyValueAndDetectChanges(o =>
                {
                    _gatewayProviderTypeFieldKey = value;
                    return _gatewayProviderTypeFieldKey;
                }, _gatewayProviderTypeFieldKey, GatewayTypeFieldKeySelector); 
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
            get { return EnumTypeFieldConverter.GatewayProvider.GetTypeField(_gatewayProviderTypeFieldKey); }
            set
            {
                var reference = EnumTypeFieldConverter.GatewayProvider.GetTypeField(value);
                if (!ReferenceEquals(TypeFieldMapperBase.NotFound, reference))
                {
                    // call through the property to flag the dirty property
                    GatewayProviderTypeFieldKey = reference.TypeKey;
                }
            }
        }

        /// <summary>
        /// Creates an in
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="settings"></param>
        /// <returns></returns>
        public override IGatewayProvider CreateInstance<T>(IDictionary<string, string> settings)
        {
            if (_gatewayProvider != null) return _gatewayProvider as T;
            return null;
        }
    }
}