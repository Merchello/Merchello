namespace Merchello.Core.Models.Interfaces
{
    using System;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Defines detached content.
    /// </summary>
    public interface IDetachedContentType : IEntity
    {        
        /// <summary>
        /// Gets or sets the entity type field key.
        /// </summary>
        Guid EntityTfKey { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the content type id.
        /// </summary>
        int? ContentTypeId { get; set; }
    }
}