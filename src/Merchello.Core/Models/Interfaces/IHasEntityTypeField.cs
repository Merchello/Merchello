namespace Merchello.Core.Models.Interfaces
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines a class that exposes an entity type field key.
    /// </summary>
    public interface IHasEntityTypeField
    {
        /// <summary>
        /// Gets or sets the entity type field key.
        /// </summary>
        [DataMember]
        Guid EntityTfKey { get; set; } 
    }
}