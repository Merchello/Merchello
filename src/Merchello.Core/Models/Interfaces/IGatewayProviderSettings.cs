using System;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Services;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a Gateway Provider
    /// </summary>
    public interface IGatewayProviderSettings : IHasExtendedData, IEntity
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
        /// The description of the provider
        /// </summary>
        [DataMember]
        string Description { get; set; }

        /// <summary>
        /// True/false indicating whether or the ExtendedData collection should be encrypted before persisted.
        /// </summary>
        [DataMember]
        bool EncryptExtendedData { get; set; }

        /// <summary>
        /// True/false indicating whether or not this provider is a "registered" and active provider.
        /// </summary>
        /// <remarks>
        /// Any provider returned from the <see cref="GatewayProviderService"/> would be an active provider
        /// </remarks>
        bool Activated { get; }

        /// <summary>
        /// Enum type of the Gateway Provider
        /// </summary>
        [DataMember]
        GatewayProviderType GatewayProviderType { get; }


    }
}