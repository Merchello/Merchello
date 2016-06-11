namespace Merchello.Web.Models.Interfaces
{
    using System;

    /// <summary>
    /// Defines a display entity.
    /// </summary>
    public interface IEntityDisplay
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        Guid Key { get; set; } 
    }
}