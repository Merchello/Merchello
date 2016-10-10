namespace Merchello.Core.Models.EntityBase
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines an Entity.
    /// Entities should always have an Id, Created and Modified date
    /// </summary>
    public interface IEntity : IHasKeyId, ITracksDirty, IDateStamped
    {
        /// <summary>
        /// Gets or sets the GUID based Id
        /// </summary>
        [DataMember]
        new Guid Key { get; set; }

        /// <summary>
        /// Gets a value indicating whether the current entity has an identity, e.g. Id.
        /// </summary>
        [IgnoreDataMember]
        bool HasIdentity { get; }            
    }
}