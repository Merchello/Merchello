namespace Merchello.Core.Models.Rdbms
{
    using System;

    using JetBrains.Annotations;

    /// <summary>
    /// The table definition and "POCO" for database operations associated with the "merchNote" table.
    /// </summary>
    internal class NoteDto : IEntityDto, IPageableDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the entity key.
        /// </summary>
        public Guid EntityKey { get; set; }

        /// <summary>
        /// Gets or sets the reference type.
        /// </summary>
        public Guid EntityTfKey { get; set; }

        /// <summary>
        /// Gets or sets the author.
        /// </summary>
        [CanBeNull]
        public string Author { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        [CanBeNull]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the note should be for internal use only.
        /// </summary>
        public bool InternalOnly { get; set; }
        
        /// <inheritdoc/>
        public DateTime UpdateDate { get; set; }

        /// <inheritdoc/>
        public DateTime CreateDate { get; set; }
    }
}