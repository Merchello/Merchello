using System;
using System.Runtime.Serialization;
using Merchello.Core.Gateway;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models.GatewayProviders
{
    public interface IRegisteredGatewayProvider : IKeyEntity
    {

        /// <summary>
        /// The GatewayProviderTypeFieldKey
        /// </summary>
        [DataMember]
        Guid GatewayProviderTypeFieldKey { get; set; }

        /// <summary>
        /// The name of the gateway provider
        /// </summary>
        [DataMember]
        string Name { get; set; }

        /// <summary>
        /// The Type.FullName of the gateway provider used to instantiate the provider
        /// </summary>
        [DataMember]
        string TypeFullName { get; set; }

        /// <summary>
        /// The configuration data associated with the gateway provider stored in xml format
        /// </summary>
        [DataMember]
        string ConfigurationData { get; set; }

        /// <summary>
        /// True/false indicating whether or not the configuration data should be encrypted when persisted
        /// </summary>
        [DataMember]
        bool EncryptConfigurationData { get; set; }

        /// <summary>
        /// The gateway provider type
        /// </summary>
        [DataMember]
        GatewayProviderType GatewayProviderType { get; set; }

    }
}