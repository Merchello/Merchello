using System;
using System.Runtime.Serialization;

namespace Merchello.Core.Models.EntityBase
{
    /// <summary>
    /// Defines the base of all Entity objects
    /// </summary>
    public interface IEntity
    {
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
        /// Indicates whether the current entity has an identity, eg. Id. or Pk.
        /// </summary>
        [IgnoreDataMember]
        bool HasIdentity { get; }
      
    }
}
