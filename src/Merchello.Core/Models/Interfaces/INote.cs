namespace Merchello.Core.Models
{
    using System;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Defines a note.
    /// </summary>
    public interface INote : IEntity
    {
        /// <summary>
        /// Gets or sets the entity key related to the note
        /// </summary>
        [DataMember]
        Guid EntityKey { get; set; }

        /// <summary>
        /// Gets or sets the entity type field key.
        /// </summary>
        [DataMember]
        Guid EntityTfKey { get; set; }

        /// <summary>
        /// Gets or sets the author.
        /// </summary>
        [DataMember]
        string Author { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        [DataMember]
        string Message { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the not is for internal use only.
        /// </summary>
        [DataMember]
        bool InternalOnly { get; set; }

    }
}