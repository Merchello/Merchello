namespace Merchello.Core.Models.EntityBase
{
    using System;

    /// <summary>
    /// Represents an entity that is date stamped.
    /// </summary>
    public interface IDateStamped
    {
        /// <summary>
        /// Gets or sets the Created Date
        /// </summary>
        DateTime CreateDate { get; set; }

        /// <summary>
        /// Gets or sets the Modified Date
        /// </summary>
        DateTime UpdateDate { get; set; }
    }
}