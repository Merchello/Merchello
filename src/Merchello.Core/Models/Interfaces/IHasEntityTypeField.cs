namespace Merchello.Core.Models
{
    using System;
    

    /// <summary>
    /// Marker interface for classes that have an entity type field.
    /// </summary>
    public interface IHasEntityTypeField
    {
        /// <summary>
        /// Gets or sets the entity type field key.
        /// </summary>
        
        Guid EntityTfKey { get; set; } 
    }
}