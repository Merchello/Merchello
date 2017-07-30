namespace Merchello.Core.Models.EntityBase
{
    using System;

    /// <summary>
    /// Defines a Merchello entity.
    /// </summary>
    public abstract class Entity : IEntity
    {
        /// <summary>
        /// Gets or sets the GUID based Id
        /// </summary>
        public virtual Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the Created Date
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Gets or sets the Modified Date
        /// </summary>
        public DateTime UpdateDate { get; set; }
    }
}
