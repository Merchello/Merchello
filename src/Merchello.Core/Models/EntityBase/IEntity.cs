namespace Merchello.Core.Models.EntityBase
{
    using System;

    /// <summary>
    /// Defines an Entity.
    /// Entities should always have an Id, Created and Modified date
    /// </summary>
    public interface IEntity : IHasKeyId, IDateStamped
    {
        /// <summary>
        /// Gets or sets the GUID based Id
        /// </summary>
        new Guid Key { get; set; }
    }
}