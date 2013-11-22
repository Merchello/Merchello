using System;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models.Interfaces
{
    /// <summary>
    /// Defines a Gateway Provider
    /// </summary>
    public interface IGatewayProviderBase : IEntity
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
        ExtendedDataCollection ExtendedData { get; set; }
    }
}