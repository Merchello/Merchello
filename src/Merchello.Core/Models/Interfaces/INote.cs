namespace Merchello.Core.Models
{
    using System;
    

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Represents a note.
    /// </summary>
    public interface INote : IEntity, IShallowClone
    {
        /// <summary>
        /// Gets or sets the entity key related to the note
        /// </summary>
        
        Guid EntityKey { get; set; }

        /// <summary>
        /// Gets or sets the entity type field key.
        /// </summary>
        
        Guid EntityTfKey { get; set; }

        /// <summary>
        /// Gets or sets the author.
        /// </summary>
        
        string Author { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        
        string Message { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the not is for internal use only.
        /// </summary>
        
        bool InternalOnly { get; set; }

    }
}