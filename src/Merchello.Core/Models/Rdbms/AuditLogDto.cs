namespace Merchello.Core.Models.Rdbms
{
    using System;

    /// <summary>
    /// The table definition and "POCO" for database operations associated with the "merchAuditLog" table.
    /// </summary>
    internal class AuditLogDto : IEntityDto, IPageableDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the entity key.
        /// </summary>
        public Guid? EntityKey { get; set; }

        /// <summary>
        /// Gets or sets the reference type.
        /// </summary>
        public Guid? EntityTfKey { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the verbosity level. 
        /// </summary>
        public int Verbosity { get; set; }

        /// <summary>
        /// Gets or sets the extended data collection.
        /// </summary>
        public string ExtendedData { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is an error record.
        /// </summary>
        public bool IsError { get; set; }


        /// <inheritdoc/>
        public DateTime UpdateDate { get; set; }

        /// <inheritdoc/>
        public DateTime CreateDate { get; set; }
    }
}