using System;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a Gateway Provider
    /// </summary>
    public interface IGatewayProvider : IEntity
    {
        /// <summary>
        /// The type field key for the provider
        /// </summary>
        [DataMember]
        Guid ProviderTfKey { get; set; }

        /// <summary>
        /// The name of the provider
        /// </summary>
        [DataMember]
        string Name { get; set; }

        /// <summary>
        /// The full Type name 
        /// </summary>
        [DataMember]
        string TypeFullName { get; set; }

        /// <summary>
        /// Extended data for the provider
        /// </summary>
        [DataMember]
        ExtendedDataCollection ExtendedData { get; }

        /// <summary>
        /// True/false indicating whether or the ExtendedData collection should be encrypted before persisted.
        /// </summary>
        [DataMember]
        bool EncryptExtendedData { get; set; }

        /// <summary>
        /// Enum type of the Gateway Provider
        /// </summary>
        [DataMember]
        GatewayProviderType GatewayProviderType { get; }


    }
}