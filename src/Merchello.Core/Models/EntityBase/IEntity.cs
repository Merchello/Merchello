using System;
using System.Runtime.Serialization;

namespace Merchello.Core.Models.EntityBase
{
    /// <summary>
    /// Defines an Entity.
    /// Entities should always have an Id, Created and Modified date
    /// </summary>
    public interface IEntity : IEntityBase
    {        
        /// <summary>
        /// Guid based Id
        /// </summary>
        [DataMember]
        Guid Key { get; set; }

       
    }
}