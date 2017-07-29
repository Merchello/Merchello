namespace Merchello.Core.Models.EntityBase
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Represents an entity that is marked with a version GUID.
    /// </summary>
    public abstract class VersionTaggedEntity : Entity, IVersionTaggedEntity
    {
        /// <summary>
        /// Gets or sets the version key.
        /// </summary>
        public Guid VersionKey { get; set; }
    }
}