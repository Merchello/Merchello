namespace Merchello.Core.Models
{
    using System;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Services;

    /// <summary>
    /// Defines a Gateway Provider
    /// </summary>
    public interface IGatewayProviderSettings : IHasExtendedData, IEntity
    {
        /// <summary>
        /// Gets or sets the type field key for the provider
        /// </summary>
        [DataMember]
        Guid ProviderTfKey { get; set; }

        /// <summary>
        /// Gets or sets the name of the provider
        /// </summary>
        [DataMember]
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the provider
        /// </summary>
        [DataMember]
        string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or the ExtendedData collection should be encrypted before persisted.
        /// </summary>
        [DataMember]
        bool EncryptExtendedData { get; set; }

        /// <summary>
        /// Gets a value indicating whether or not this provider is a "registered" and active provider.
        /// </summary>
        /// <remarks>
        /// Any provider returned from the <see cref="GatewayProviderService"/> would be an active provider
        /// </remarks>
        bool Activated { get; }

        /// <summary>
        /// Gets the type of the Gateway Provider
        /// </summary>
        [DataMember]
        GatewayProviderType GatewayProviderType { get; }


    }
}