using System;
using System.Runtime.Serialization;

namespace Merchello.Core.Models.EntityBase
{
    /// <summary>
    /// Defines an Entity.
    /// Entities should always have an Id, Created and Modified date
    /// </summary>
    public interface IEntity : ITracksDirty
    {        
        /// <summary>
        /// Guid based Id
        /// </summary>
        [DataMember]
        Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the Created Date
        /// </summary>
        [DataMember]
        DateTime CreateDate { get; set; }

        /// <summary>
        /// Gets or sets the Modified Date
        /// </summary>
        [DataMember]
        DateTime UpdateDate { get; set; }

        /// <summary>
        /// Indicates whether the current entity has an identity, eg. Id.
        /// </summary>
        [IgnoreDataMember]
        bool HasIdentity { get; }
    }
}