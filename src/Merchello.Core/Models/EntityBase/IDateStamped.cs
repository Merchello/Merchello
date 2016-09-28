namespace Merchello.Core.Models.EntityBase
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents an entity that is date stamped.
    /// </summary>
    public interface IDateStamped
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
    }
}