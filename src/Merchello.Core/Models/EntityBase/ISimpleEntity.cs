using System;
using System.Runtime.Serialization;

namespace Merchello.Core.Models.EntityBase
{
    public interface ISimpleEntity
    {
        /// <summary>
        /// The Id of the entity
        /// </summary>
        [DataMember]
        int Id { get; set; }

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

        [IgnoreDataMember]
        bool HasIdentity { get; }
    }
}